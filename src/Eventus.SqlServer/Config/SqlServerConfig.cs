using System;
using System.Collections.Generic;
using System.Reflection;

namespace Eventus.SqlServer.Config
{
    public class SqlServerConfig
    {
        public SqlServerConfig(string connectionString, Assembly domainAssembly) : this(connectionString, new List<Assembly> { domainAssembly })
        {}

        public SqlServerConfig(string connectionString, List<Assembly> domainAssemblies)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            DomainAssemblies = domainAssemblies ?? throw new ArgumentException(nameof(domainAssemblies));
            Schema = "dbo";
        }

        public string ConnectionString { get; }

        public List<Assembly> DomainAssemblies { get; }

        public string Schema { get; set; }
    }
}