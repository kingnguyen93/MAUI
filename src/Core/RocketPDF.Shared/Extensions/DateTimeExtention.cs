namespace System
{
    public static class DateTimeExtention
    {
        public static DateOnly ToDateOny(this DateTime dateTime)
        {
            return DateOnly.FromDateTime(dateTime);
        }

        public static DateTime ToDateTime(this DateOnly date)
        {
            return date.ToDateTime(TimeOnly.MinValue);
        }
    }
}