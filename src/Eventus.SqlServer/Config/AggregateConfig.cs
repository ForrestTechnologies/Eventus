using System;
using System.Reflection;

namespace Eventus.SqlServer.Config
{
    public class AggregateConfig
    {
        public AggregateConfig(TypeInfo aggregateType)
        {
            AggregateType = aggregateType ?? throw new ArgumentNullException(nameof(aggregateType));
        }

        public TypeInfo AggregateType { get; }
    }
}