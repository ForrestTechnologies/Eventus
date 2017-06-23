using System;
using System.Collections.Generic;
using System.Reflection;
using Eventus.Samples.Core.Domain;
using FluentAssertions;
using Xunit;

namespace Eventus.Tests.Unit
{
    public class AggregateHelperTests
    {
        [Fact]
        public void GetAggregateTypes_should_throw_exception_if_assemblies_is_null()
        {
            Action action = () => AggregateHelper.GetAggregateTypes((Assembly)null);

            action.ShouldThrow<ArgumentNullException>();
        }


        [Fact]
        public void GetAggregateTypes_should_throw_exception_if_assemblies_is_empty()
        {
            Action action = () => AggregateHelper.GetAggregateTypes(new List<Assembly>());

            action.ShouldThrow<ArgumentException>();
        }

        [Fact]
        public void GetAggregateTypes_should_return_all_aggregates()
        {
            var aggregates = AggregateHelper.GetAggregateTypes(Assembly.GetAssembly(typeof(BankAccount))
            );

            aggregates.Should().Contain(new List<Type> {typeof(BankAccount)});
        }
    }
}