using System;
using System.Configuration;
using System.Threading.Tasks;
using Eventus.Samples.Infrastructure;
using Eventus.Samples.Infrastructure.Factories;
using Eventus.Samples.Infrastructure.Factories.Providers;
using Xunit;

namespace Eventus.Tests.Integration
{
    public class StorageProviderFixture : IDisposable
    {
        public StorageProviderFixture()
        {
            SetupAsync().Wait();
            ProviderFactory.SetProvider(ConfigurationManager.AppSettings[Constants.Provider].ToUpperInvariant());
            //todo find a nicer way to do this
            SqlServerProviderFactory.ConnectionString = ConfigurationManager.ConnectionStrings["Eventus"].ToString();
        }

        private static Task SetupAsync()
        {
            return ProviderFactory.Current.InitAsync();
        }

        public void Dispose()
        {
            var cleaner = ProviderFactory.Current.CreateTeardownAsync().Result;
            cleaner.TearDownAsync().Wait();
        }
    }

    [CollectionDefinition(Name)]
    public class StorageProvidersCollection : ICollectionFixture<StorageProviderFixture>
    {
        // This class has no code, and is never created. Its purpose is simply
        // to be the place to apply [CollectionDefinition] and all the
        // ICollectionFixture<> interfaces.

        public const string Name = "DocumentDb collection";
    }
}