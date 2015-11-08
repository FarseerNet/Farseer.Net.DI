using FS.DI.Core;
using System;
using System.Collections.Generic;
 
namespace FS.DI.Resolve
{
    /// <summary>
    /// 依赖缓存表
    /// </summary>
    internal interface IDependencyTable : IDisposable
    {
        IDictionary<Type, DependencyEntry> DependencyEntryTable { get; }

        IDictionary<Type, DependencyEntry> PropertyEntryTable { get; }

        IDictionary<DependencyEntry, bool> HasPropertyEntryTable { get; }

        IDictionary<Tuple<IScopedResolver, DependencyEntry>, Object> ScopedTable { get; }

        IDictionary<DependencyEntry, Func<IDependencyResolver, Object[], Object>> CompileTable { get; }
    }
}
  