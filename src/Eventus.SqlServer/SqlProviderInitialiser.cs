using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Dapper;
using Eventus.SqlServer.Config;
using Eventus.Storage;

namespace Eventus.SqlServer
{
    public class SqlProviderInitialiser : SqlServerProviderBase, IStorageProviderInitialiser
    {
        private readonly SqlServerConfig _config;

        public SqlProviderInitialiser(SqlServerConfig config) : base(config.ConnectionString)
        {
            _config = config;
        }

        public async Task InitAsync()
        {
            var aggregateTypes = DetectAggregates();

            var aggregates = BuildAggregateConfigs(aggregateTypes);

            var connection = await GetOpenConnectionAsync().ConfigureAwait(false);

            using (connection)
            {
                using (var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted))
                {
                    foreach (var aggregateConfig in aggregates)
                    {
                        await CreateTableForAggregateAsync(connection, aggregateConfig).ConfigureAwait(false);

                        await CreateSnapshotTableForAggregateAsync(connection, aggregateConfig).ConfigureAwait(false);
                    }

                    transaction.Commit();
                }
            }
        }

        protected virtual IEnumerable<TypeInfo> DetectAggregates()
        {
            var assemblies = _config.DomainAssemblies;

            return AggregateHelper.GetAggregateTypes(assemblies);
        }

        protected virtual IEnumerable<AggregateConfig> BuildAggregateConfigs(IEnumerable<TypeInfo> aggregateTypes)
        {
            var aggregateConfigs = aggregateTypes.Select(t => new AggregateConfig(t));

            return aggregateConfigs;
        }

        protected virtual Task CreateTableForAggregateAsync(IDbConnection connection, AggregateConfig aggregateConfig)
        {
            var aggregateTable = string.Format(@"IF OBJECT_ID (N'{0}', N'U') IS NOT NULL

                                             DROP TABLE [{1}].[{0}]

                                             CREATE TABLE [{1}].[{0}](
	                                            [Id]               UNIQUEIDENTIFIER NOT NULL,
                                                [AggregateId]      UNIQUEIDENTIFIER NOT NULL,
                                                [TargetVersion]    INT              NOT NULL,
                                                [ClrType]          NVARCHAR (500)   NOT NULL,
                                                [AggregateVersion] INT              NOT NULL,
                                                [TimeStamp]        DATETIME2 (7)    NOT NULL,
                                                [Data]             NVARCHAR (MAX)   NOT NULL
	                                            PRIMARY KEY (AggregateId,[Id]))", TableName(aggregateConfig.AggregateType), _config.Schema);

            return connection.ExecuteAsync(aggregateTable);
        }

        protected virtual Task CreateSnapshotTableForAggregateAsync(IDbConnection connection, AggregateConfig aggregateConfig)
        {
            var aggregateTable = string.Format(@"IF OBJECT_ID (N'{0}', N'U') IS NOT NULL

                                             DROP TABLE [{1}].[{0}]

                                             CREATE TABLE [{1}].[{0}](
	                                            [Id]               UNIQUEIDENTIFIER NOT NULL,
                                                [AggregateId]      UNIQUEIDENTIFIER NOT NULL,                                                
                                                [ClrType]          NVARCHAR (500)   NOT NULL,
                                                [AggregateVersion] INT              NOT NULL,
                                                [TimeStamp]        DATETIME2 (7)    NOT NULL,
                                                [Data]             NVARCHAR (MAX)   NOT NULL
	                                            PRIMARY KEY (AggregateId,[Id]))", SnapshotTableName(aggregateConfig.AggregateType), _config.Schema);

            return connection.ExecuteAsync(aggregateTable);
        }
    }
}