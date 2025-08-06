using System.Data;
using System.Reflection;

namespace Sediin.Core.Mvc.Helpers.ReflectionHelpers
{
    public static class Reflection
    {
        public static DataTable ListToDataTable<T>(IEnumerable<T> items)
        {
            var dataTable = new DataTable(typeof(T).Name);

            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in Props)
            {
                var colType = prop.PropertyType;
                if (colType.IsGenericType && colType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    colType = Nullable.GetUnderlyingType(colType);

                dataTable.Columns.Add(prop.Name, colType);
            }

            foreach (var item in items)
            {
                var values = new object[Props.Length];
                for (int i = 0; i < Props.Length; i++)
                {
                    values[i] = Props[i].GetValue(item) ?? DBNull.Value;
                }
                dataTable.Rows.Add(values);
            }

            return dataTable;
        }
    }
}
