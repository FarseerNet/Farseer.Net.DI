using System;
using System.Collections.Generic;

namespace FS.Extends
{
    public static class EnumerableExtends
    {
        /// <summary>
        ///     通过使用指定的委托对值进行比较返回序列中的非重复元素
        /// </summary>
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> seenKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (seenKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        ///     对集合中的每个元素执行指定操作
        /// </summary>
        public static IEnumerable<TSource>  ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            foreach (var item in source)
                action(item);
            return source;
        }

        /// <summary>
        ///     对集合中的每个元素执行指定操作
        /// </summary>
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource, int> action)
        {
            int index = 0;
            foreach (var item in source)
                action(item, index++);
            return source;
        }
    }
}
