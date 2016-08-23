using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FS.Extends
{
    public static class EnumerableExtends
    {
        /// <summary>
        ///     通过使用指定的委托对值进行比较返回序列中的非重复元素
        /// </summary>
        public static IEnumerable<TSource> Distinct<TSource, TKey>(this IEnumerable<TSource> source,
            Func<TSource, TKey> keySelector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (keySelector == null)
                throw new ArgumentNullException(nameof(keySelector));
            var seenKeys = new HashSet<TKey>();
            return source.Where(element => seenKeys.Add(keySelector(element)));
        }

        /// <summary>
        ///     对集合中的每个元素执行指定操作
        /// </summary>
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source, Action<TSource> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            var items = source as TSource[] ?? source.ToArray();
            foreach (var item in items)
                action(item);
            return items;
        }

        /// <summary>
        ///     对集合中的每个元素执行指定操作
        /// </summary>
        public static IEnumerable<TSource> ForEach<TSource>(this IEnumerable<TSource> source,
            Action<TSource, int> action)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (action == null)
                throw new ArgumentNullException(nameof(action));
            var index = 0;
            var items = source as TSource[] ?? source.ToArray();
            foreach (var item in items)
                action(item, index++);
            return items;
        }

        /// <summary>
        ///      根据指定类型和键筛选 System.Collections.IEnumerable 的元素。
        /// </summary>
        public static IEnumerable<TResult> OfType<TSource, TResult>(this IEnumerable<TSource> source,
            Func<TSource, object> iterator)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (iterator == null)
                throw new ArgumentNullException(nameof(iterator));
            return source.Where(obj => iterator(obj) is TResult).Select(iterator).OfType<TResult>();
        }

        /// <summary>
        ///      根据指定类型，键和返回类型筛选 System.Collections.IEnumerable 的元素。
        /// </summary>
        public static IEnumerable<TResult> OfType<TSource, TIterator, TResult>(this IEnumerable<TSource> source,
            Func<TSource, object> iterator, Func<TSource, TResult> selector)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (iterator == null)
                throw new ArgumentNullException(nameof(iterator));
            if (selector == null)
                throw new ArgumentNullException(nameof(selector));
            return source.Where(obj => iterator(obj) is TIterator).Select(selector);
        }

    }
}
