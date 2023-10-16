namespace RocketPDF.EntityFrameworkCore.Helpers
{
    public static class StringQueryHelper
    {
        public static string InConditionParam(string column, object[] values)
        {
            return $"{column} IN ({string.Join(",", values.Select(x => $"''{x}''"))})";
        }

        public static string InConditionQuery(string column, object[] values)
        {
            return $"{column} IN ({string.Join(",", values.Select(x => $"'{x}'"))})";
        }

        public static string InConditionOrFalseParam(string column, object[] values)
        {
            return values.Length > 0 ? $"{column} IN ({string.Join(",", values.Select(x => $"''{x}''"))})" : "1=0";
        }

        public static string InConditionOrFalseQuery(string column, object[] values)
        {
            return values.Length > 0 ? $"{column} IN ({string.Join(",", values.Select(x => $"'{x}'"))})" : "1=0";
        }
    }
}