using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eventus.Events;
using Eventus.Exceptions;

namespace Eventus
{
    internal static class ReflectionHelper
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<Type, MethodInfo>> AggregateEventHandlerCache =
                new ConcurrentDictionary<Type, ConcurrentDictionary<Type, MethodInfo>>();

        internal static Dictionary<Type, MethodInfo> FindEventHandlerMethodsInAggregate(Type aggregateType)
        {
            if (AggregateEventHandlerCache.ContainsKey(aggregateType) == false)
            {
                var eventHandlers = new ConcurrentDictionary<Type, MethodInfo>();

                var methods = aggregateType.GetTypeInfo().GetMethodsBySig(typeof(void), true, typeof(IEvent)).ToList();

                if (methods.Any())
                {
                    foreach (var m in methods)
                    {
                        var parameter = m.GetParameters().First();
                        if (eventHandlers.TryAdd(parameter.ParameterType, m) == false)
                        {
                            throw new AggregateMethodException($"Multiple methods found handling same event in {aggregateType.Name}");
                        }
                    }
                }

                if (AggregateEventHandlerCache.TryAdd(aggregateType, eventHandlers) == false)
                {
                    throw new AggregateMethodException($"Error registering methods for handling events in {aggregateType.Name}");
                }
            }


            return AggregateEventHandlerCache[aggregateType].ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }

        internal static IEnumerable<MethodInfo> GetMethodsBySig(this TypeInfo type,
            Type returnType,
            bool matchParameterInheritance,
            params Type[] parameterTypes)
        {

            return type.DeclaredMethods.Where(m =>
            {
                //ignore properties
                if (m.Name.StartsWith("get_", StringComparison.OrdinalIgnoreCase) ||
                    m.Name.StartsWith("set_", StringComparison.OrdinalIgnoreCase))
                    return false;

                //does the return type match
                if (m.ReturnType != returnType)
                    return false;

                //does the method have the same number of parameters that are either the same type or assignable from the passed in parameter types 
                //based on the matchParameterInheritance switch
                var parameters = m.GetParameters();

                if (parameterTypes == null || parameterTypes.Length == 0)
                    return parameters.Length == 0;

                if (parameters.Length != parameterTypes.Length)
                    return false;

                return !parameterTypes.Where((t, i) => (parameters[i].ParameterType == t || matchParameterInheritance && t.GetTypeInfo().IsAssignableFrom(parameters[i].ParameterType.GetTypeInfo())) == false).Any();
            });
        }
    }
}