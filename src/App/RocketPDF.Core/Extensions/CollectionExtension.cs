namespace RocketPDF.Core.Extensions
{
    public static class CollectionExtension
    {
        /// <summary>
        /// Checks whatever given collection object is null or has no item.
        /// </summary>
        public static bool IsNullOrEmpty<T>(this ICollection<T> source)
        {
            return source == null || source.Count == 0;
        }

        /// <summary>
        /// Adds an item to the collection if it's not already in the collection.
        /// </summary>
        /// <param name="source">Collection</param>
        /// <param name="item">Item to check and add</param>
        /// <typeparam name="T">Type of the items in the collection</typeparam>
        /// <returns>Returns True if added, returns False if not.</returns>
        public static bool AddIfNotContains<T>(this ICollection<T> source, T item)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (source.Contains(item))
            {
                return false;
            }

            source.Add(item);
            return true;
        }

        public static void AddRange<T>(this ICollection<T> destination, IEnumerable<T> source)
        {
            if (destination is List<T> list)
            {
                list.AddRange(source);
            }
            else
            {
                foreach (T item in source)
                {
                    destination.Add(item);
                }
            }
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            if (source == null) return;
            foreach (var item in source) action(item);
        }

        public static IEnumerable<TSource> WhereIf<TSource>(this IEnumerable<TSource> source, bool condition, Func<TSource, bool> predicate)
        {
            return condition ? source.Where(predicate) : source;
        }

        public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dict, K key)
        {
            if (dict == null) return default;
            return dict.GetValueOrDefault(key, default(V));
        }

        public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dict, K key, V defVal)
        {
            if (dict == null) return defVal;
            return dict.GetValueOrDefault(key, () => defVal);
        }

        public static V GetValueOrDefault<K, V>(this IDictionary<K, V> dict, K key, Func<V> defValSelector)
        {
            if (dict == null) return defValSelector();
            return dict.TryGetValue(key, out V value) ? value : defValSelector();
        }
    }
}