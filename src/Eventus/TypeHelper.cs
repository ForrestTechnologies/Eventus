using System.Reflection;

namespace Eventus
{
    public class TypeHelper
    {
        public static string GetClrTypeName(object item)
        {
            return item.GetType() + "," + item.GetType().GetTypeInfo().Assembly.GetName().Name;
        }
    }
}