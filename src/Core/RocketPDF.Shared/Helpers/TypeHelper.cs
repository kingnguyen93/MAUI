using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using RocketPDF.Shared.Extensions;

namespace RocketPDF.Shared.Helpers
{
    public static class TypeHelper
    {
        public static string GetFieldName<TEntity>(string column)
        {
            return GetFieldName(typeof(TEntity), column);
        }

        public static string GetFieldName(Type type, string column)
        {
            var pro = Array.Find(type.GetProperties(), x => x.GetCustomAttributes<ColumnAttribute>().Any(a => a.Name == column));
            if (pro != null)
                return pro?.Name;
            return column.ToCamelCase();
        }
    }
}