using System;
using System.Data.SqlClient;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Eventus.SqlServer
{
    public abstract class SqlServerProviderBase
    {
        private static JsonSerializerSettings _serializerSetting;
        protected static JsonSerializerSettings SerializerSettings
        {
            get
            {
                if (_serializerSetting != null)
                    return _serializerSetting;

                _serializerSetting = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.None,
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                _serializerSetting.Converters.Add(new StringEnumConverter());

                return _serializerSetting;
            }
        }

        protected string ConnectionString;

        protected async Task<SqlConnection> GetOpenConnectionAsync()
        {
            var connection = new SqlConnection(ConnectionString);
            await connection.OpenAsync().ConfigureAwait(false);
            return connection;
        }

        protected SqlServerProviderBase(string connectionString)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected static string GetClrTypeName(object item)
        {
            return TypeHelper.GetClrTypeName(item);
        }

        protected static string TableName(Type aggregateType)
        {
            return TableName(aggregateType.GetTypeInfo());
        }


        protected static string TableName(TypeInfo aggregateType)
        {
            return aggregateType.Name;
        }

        protected static string SnapshotTableName(Type aggregateType)
        {
            return SnapshotTableName(aggregateType.GetTypeInfo());
        }

        protected static string SnapshotTableName(TypeInfo aggregateType)
        {
            return $"{aggregateType.Name}_Snapshot";
        }
    }
}