using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Eventus.Cleanup;
using Eventus.EventBus;
using Eventus.Logging;
using Eventus.Samples.Infrastructure.Factories.Providers;
using Eventus.Storage;

namespace Eventus.Samples.Infrastructure.Factories
{
    public abstract class ProviderFactory
    {
        public static readonly ProviderFactory SqlServer = new SqlServerProviderFactory(0, "SqlServer");
        public static readonly ProviderFactory DocumentDb = new DocumentDbProviderFactory(1, "DocumentDb");
        public static readonly ProviderFactory EventStore = new EventProviderFactory(2, "EventStore");

        public string Name { get; }
        public int Value { get; }

        protected ProviderFactory(int value, string name)
        {
            Value = value;
            Name = name;
        }

        public static ProviderFactory Current => FromString(ConfigurationManager.AppSettings[Constants.Provider].ToLowerInvariant());

        public static IEnumerable<ProviderFactory> List()
        {
            return new[] { SqlServer, DocumentDb, EventStore };
        }

        public static ProviderFactory FromString(string roleString)
        {
            return List().Single(r => string.Equals(r.Name, roleString, StringComparison.OrdinalIgnoreCase));
        }

        public Task<IRepository> CreateRepositoryAsync()
        {
            return CreateRepositoryAsync(null);
        }

        public virtual async Task<IRepository> CreateRepositoryAsync(IEventPublisher eventPublisher)
        {
            var repo = new RepositoryLoggingDecorator(new Repository(
                    await CreateEventStorageProviderAsync().ConfigureAwait(false),
                    await CreateSnapshotStorageProviderAsync().ConfigureAwait(false),
                eventPublisher));

            return repo;
        }

        public abstract Task<ITeardown> CreateTeardownAsync();

        public abstract Task<IEventStorageProvider> CreateEventStorageProviderAsync();

        public abstract Task<ISnapshotStorageProvider> CreateSnapshotStorageProviderAsync();

        public abstract Task InitAsync();
    }
}