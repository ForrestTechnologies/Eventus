using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Eventus.SqlServer.Teardown
{
    internal class SqlServerDbAdapter : IDbAdapter
    {
        public string BuildTableCommandText(Checkpoint checkpoint)
        {
            var commandText = @"select s.name, t.name 
                                   from sys.tables t INNER JOIN sys.schemas s ON t.schema_id = s.schema_id
                                   WHERE s.principal_id = '1'";

            if (checkpoint.TablesToIgnore.Any())
            {
                var args = string.Join(",", checkpoint.TablesToIgnore.Select(t => $"N'{t}'"));

                commandText += " AND t.name NOT IN (" + args + ")";
            }
            if (checkpoint.SchemasToExclude.Any())
            {
                var args = string.Join(",", checkpoint.SchemasToExclude.Select(t => $"N'{t}'"));

                commandText += " AND s.name NOT IN (" + args + ")";
            }
            else if (checkpoint.SchemasToInclude.Any())
            {
                var args = string.Join(",", checkpoint.SchemasToInclude.Select(t => $"N'{t}'"));

                commandText += " AND s.name IN (" + args + ")";
            }

            return commandText;
        }

        public string BuildRelationshipCommandText(Checkpoint checkpoint)
        {
            string commandText = @"select
                                       pk_schema.name, so_pk.name,
                                       fk_schema.name, so_fk.name
                                    from
                                    sysforeignkeys sfk
	                                    inner join sys.objects so_pk on sfk.rkeyid = so_pk.object_id
	                                    inner join sys.schemas pk_schema on so_pk.schema_id = pk_schema.schema_id
	                                    inner join sys.objects so_fk on sfk.fkeyid = so_fk.object_id			
	                                    inner join sys.schemas fk_schema on so_fk.schema_id = fk_schema.schema_id
                                    where 1=1";

            if (checkpoint.TablesToIgnore != null && checkpoint.TablesToIgnore.Any())
            {
                var args = string.Join(",", checkpoint.TablesToIgnore.Select(t => $"N'{t}'"));

                commandText += " AND so_pk.name NOT IN (" + args + ")";
            }
            if (checkpoint.SchemasToExclude != null && checkpoint.SchemasToExclude.Any())
            {
                var args = string.Join(",", checkpoint.SchemasToExclude.Select(t => $"N'{t}'"));

                commandText += " AND pk_schema.name NOT IN (" + args + ")";
            }
            else if (checkpoint.SchemasToInclude != null && checkpoint.SchemasToInclude.Any())
            {
                var args = string.Join(",", checkpoint.SchemasToInclude.Select(t => $"N'{t}'"));

                commandText += " AND pk_schema.name IN (" + args + ")";
            }

            return commandText;
        }

        public string BuildDeleteCommandText(IEnumerable<string> tablesToDelete)
        {
            var builder = new StringBuilder();

            foreach (var tableName in tablesToDelete)
            {
                builder.Append($"delete from {tableName};\r\n");
            }
            return builder.ToString();
        }
    }
}