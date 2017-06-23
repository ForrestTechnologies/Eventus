using System.Reflection;
using System.Threading.Tasks;
using Eventus.Cleanup;
using Eventus.Samples.Core.Domain;
using Eventus.SqlServer;
using Eventus.SqlServer.Config;
using Eventus.Storage;

namespace Eventus.Samples.Infrastructure.Factories.Providers
{
    public class SqlServerProviderFactory : ProviderFactory
    {
        public static string ConnectionString { get; set; } = "Server=(localdb)\\MSSQLLocalDB;Initial Catalog=Eventus;Integrated Security=True";

        public SqlServerProviderFactory(int value, string displayName) : base(value, displayName)
        {
        }

        public override Task<ITeardown> CreateTeardownAsync()
        {
            return Task.FromResult<ITeardown>(new SqlServerTeardown(ConnectionString));
        }

        public override Task<IEventStorageProvider> CreateEventStorageProviderAsync()
        {
            return Task.FromResult<IEventStorageProvider>(new SqlServerEventStorageProvider(ConnectionString));
        }

        public override Task<ISnapshotStorageProvider> CreateSnapshotStorageProviderAsync()
        {
            return Task.FromResult<ISnapshotStorageProvider>(new SqlServerSnapshotStorageProvider(
                ConnectionString, 3));
        }

        public override Task InitAsync()
        {
            var init = new SqlProviderInitialiser(new SqlServerConfig(ConnectionString, typeof(BankAccount).GetTypeInfo().Assembly));
            return init.InitAsync();
        }
    }
}