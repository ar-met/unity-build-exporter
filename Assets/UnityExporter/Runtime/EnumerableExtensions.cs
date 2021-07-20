using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UnityExporter
{
    public static class EnumerableExtensions
    {
        public static string ElementsToString<T>(this IEnumerable<T> enumerable)
        {
            var sb = new StringBuilder();

            // making sure to start at index 0
            IEnumerable<T> enumerableList = enumerable.ToList();
            sb.Append($"count: {enumerableList.Count()}, elements: ");

            foreach (T element in enumerableList)
            {
                sb.Append($"'{element}' ");
            }

            return sb.ToString();
        }
    }
}
