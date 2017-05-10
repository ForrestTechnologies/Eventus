using System;
using System.Threading.Tasks;
using EventSourcing.DocumentDb;
using EventSourcing.Samples.Core.Domain;
using EventSourcing.Samples.Infrastructure;
using FluentAssertions;
using Xunit;

namespace EventSourcing.Tests.Integration
{
    [Collection(DocumetDbCollection.Name)]
    public class DocumentDbSnapshotProviderTest
    {
        private readonly DocumentDbSnapShotProvider _provider;

        public DocumentDbSnapshotProviderTest()
        {
            _provider = DocumentDbFactory.CreateDocumentDbSnapShotProvider();
            _provider.InitAsync(DocumentDbFactory.DocumentDbConfig).Wait();
        }

        [Fact]
        public async Task SaveSnapshotAsync_should_store_current_aggregate_state()
        {
            var aggregateId = Guid.NewGuid();
            var aggregate = new BankAccount(aggregateId, "Joe Blogs");

            var expected = aggregate.TakeSnapshot();

            await _provider.SaveSnapshotAsync(aggregate.GetType(), expected);

            var actual = await _provider.GetSnapshotAsync(aggregate.GetType(), aggregateId);

            actual.Id.Should().Be(expected.Id);
            actual.AggregateId.Should().Be(expected.AggregateId);
            actual.Version.Should().Be(expected.Version);

            actual.As<BankAccountSnapshot>().Name.Should().Be(expected.As<BankAccountSnapshot>().Name);
            actual.As<BankAccountSnapshot>().CurrentBalance.Should().Be(expected.As<BankAccountSnapshot>().CurrentBalance);
            actual.As<BankAccountSnapshot>().Transactions.Should().BeEquivalentTo(expected.As<BankAccountSnapshot>().Transactions);
        }

        [Fact]
        public async Task GetSnapshotAsync_should_get_the_latest_snapshot()
        {
            var aggregateId = Guid.NewGuid();
            var aggregate = new BankAccount(aggregateId, "Joe Blogs");

            var firstSnapshot = aggregate.TakeSnapshot();

            await _provider.SaveSnapshotAsync(aggregate.GetType(), firstSnapshot);

            aggregate.Deposit(10);

            var expected = aggregate.TakeSnapshot();

            await _provider.SaveSnapshotAsync(aggregate.GetType(), expected);

            var actual = await _provider.GetSnapshotAsync(aggregate.GetType(), aggregateId);

            actual.Id.Should().Be(expected.Id);
            actual.AggregateId.Should().Be(expected.AggregateId);
            actual.Version.Should().Be(expected.Version);

            actual.As<BankAccountSnapshot>().Name.Should().Be(expected.As<BankAccountSnapshot>().Name);
            actual.As<BankAccountSnapshot>().CurrentBalance.Should().Be(expected.As<BankAccountSnapshot>().CurrentBalance);
            actual.As<BankAccountSnapshot>().Transactions.Count.Should().Be(expected.As<BankAccountSnapshot>().Transactions.Count);
            actual.As<BankAccountSnapshot>().Transactions.Should().BeEquivalentTo(expected.As<BankAccountSnapshot>().Transactions);

        }
    }
}