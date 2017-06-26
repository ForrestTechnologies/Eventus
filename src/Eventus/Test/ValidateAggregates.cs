using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eventus.Domain;
using Eventus.Events;
using Eventus.Exceptions;

namespace Eventus.Test
{
    public static class ValidateAggregates
    {
        public static void AssertThatAggregatesSupportAllEvents(params Assembly[] domainAssemblies)
        {
            //get all events in all assemblies
            var eventType = typeof(Event).GetTypeInfo();
            var events = domainAssemblies.SelectMany(a => a.DefinedTypes)
                .Where(t => !t.Equals(eventType) && eventType.IsAssignableFrom(t));

            //get all aggregates
            var aggregateType = typeof(Aggregate).GetTypeInfo();
            var aggregates = domainAssemblies.SelectMany(a => a.DefinedTypes)
                .Where(t => !t.Equals(aggregateType) && aggregateType.IsAssignableFrom(t));

            //get aggregate apply methods
            var aggregateMethods = aggregates.SelectMany(a => a.GetMethodsBySig(typeof(void), true, typeof(IEvent)))
                .Select(m => m.GetParameters().First());

            var errors = new List<TypeInfo>();

            foreach (var @event in events)
            {
                var noMethodForType = aggregateMethods.All(m => !m.ParameterType.GetTypeInfo().Equals(@event));

                if (noMethodForType)
                {
                    errors.Add(@event);
                }
            }

            if (errors.Any())
            {
                throw new AggregateEventNotSupportException(errors);
            }
        }
    }
}