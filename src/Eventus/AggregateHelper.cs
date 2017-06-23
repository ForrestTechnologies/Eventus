using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eventus.Domain;

namespace Eventus
{
    public static class AggregateHelper
    {
        public static IEnumerable<Type> GetAggregateTypes(Assembly assembly)
        {
            return GetAggregateTypes(new List<Assembly> { assembly });
        }

        public static IEnumerable<Type> GetAggregateTypes(List<Assembly> assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
            if (assemblies.None()) throw new ArgumentException("At least one assembly must be provided", nameof(assemblies));

            var aggregateType = typeof(Aggregate);
            var types = assemblies.SelectMany(t => t.GetTypes())
                .Where(t => t != aggregateType && aggregateType.GetTypeInfo().IsSubclassOf(t));

            return types;
        }
    }
}