using System;
using System.Collections.Generic;
using System.Reflection;

namespace Eventus.DocumentDb.Config
{
    public class DocumentDbConfig
    {
        public DocumentDbConfig(string databaseId, Assembly assembly) : this(databaseId, new List<Assembly> { assembly })
        {}

        public DocumentDbConfig(string databaseId, List<Assembly> domainAssemblies)
        {
            DomainAssemblies = domainAssemblies ?? throw new ArgumentNullException(nameof(domainAssemblies));
            DatabaseId = databaseId ?? throw new ArgumentNullException(nameof(databaseId));
            DefaultThroughput = 400;
            DefaultSnapshotThroughput = 400;

            PartitionKey = "/aggregateId";
            ExcludePaths = new List<string>
            {
                "/data/*",
                "/clrType/*",
                "/targetVersion/*",
                "/timestamp/*"
            };
        }

        public string DatabaseId { get; }

        public List<Assembly> DomainAssemblies { get; }

        public int DefaultThroughput { get; set; }

        public int DefaultSnapshotThroughput { get; set; }

        public string PartitionKey { get; set; }

        public List<string> ExcludePaths { get; set; }
    }
}