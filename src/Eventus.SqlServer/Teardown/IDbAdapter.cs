using System.Collections.Generic;

namespace Eventus.SqlServer.Teardown
{
    internal interface IDbAdapter
    {
        string BuildTableCommandText(Checkpoint checkpoint);
        string BuildRelationshipCommandText(Checkpoint checkpoint);
        string BuildDeleteCommandText(IEnumerable<string> tablesToDelete);
    }
}