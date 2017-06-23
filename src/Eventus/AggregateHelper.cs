using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eventus.Domain;

namespace Eventus
{
    public static class AggregateHelper
    {
        public static IEnumerable<TypeInfo> GetAggregateTypes(Assembly assembly)
        {
            return GetAggregateTypes(new List<Assembly> { assembly });
        }

        public static IEnumerable<TypeInfo> GetAggregateTypes(List<Assembly> assemblies)
        {
            if (assemblies == null) throw new ArgumentNullException(nameof(assemblies));
            if (assemblies.None()) throw new ArgumentException("At least one assembly must be provided", nameof(assemblies));

            var aggregateType = typeof(Aggregate);
            var types = assemblies.SelectMany(a => a.DefinedTypes)
                .Where(t => !t.Equals(aggregateType.GetTypeInfo()) && t.IsSubclassOf(aggregateType));

            return types;
        }
    }
}