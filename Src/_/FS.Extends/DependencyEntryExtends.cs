using System;
using System.Collections.Generic;

namespace FS.DI.Core
{
    public static class DependencyEntryExtends
    {
        /// <summary>
        ///     返回当前依赖服务对象链上的依赖服务集合
        /// </summary>
        public static IEnumerable<DependencyEntry> Chain(this DependencyEntry dependencyEntry)
        {
            if (dependencyEntry == null) throw new ArgumentNullException(nameof(dependencyEntry));
            for (var entry = dependencyEntry; entry.Next != null; entry = entry.Next)
            {
                yield return entry;
            }
            yield return dependencyEntry.Last;
        }
    }
}
