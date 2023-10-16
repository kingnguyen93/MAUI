using RocketPDF.Infrastructure.Services;

namespace RocketPDF.Shared.Extensions
{
    public static class ObjectExtension
    {
        /// <summary>
        /// Convert object to cache keys
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="excludes"></param>
        /// <returns></returns>
        public static string ToCacheKeys<T>(this T source, params string[] excludes) where T : IBaseRequestDto
        {
            return string.Join("_", GetObjectValues(source, excludes));
        }

        /// <summary>
        /// Get object values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="excludes"></param>
        /// <returns></returns>
        public static IEnumerable<object?> GetObjectValues<T>(this T source, params string[] excludes) where T : IBaseRequestDto
        {
            return typeof(T).GetProperties().WhereIf(excludes.Length > 0, x => !excludes.Contains(x.Name)).Select(p => p.GetValue(source));
        }
    }
}