using System.Collections.Generic;
using System.Linq;

namespace Eventus
{
    internal static class Extensions
    {
        internal static bool None<T>(this IEnumerable<T> list)
        {
            return !list.Any();
        }
    }
}