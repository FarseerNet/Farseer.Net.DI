using FS.DI.Core;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
 
namespace FS.DI.Resolver
{
    /// <summary>
    /// 依赖缓存表实现
    /// </summary>
    internal class DependencyTable : IDependencyTable
    {
        public IDictionary<DependencyEntry, Func<IDependencyResolver, Object[], Object>> CompileTable { get; } = new ConcurrentDictionary<DependencyEntry, Func<IDependencyResolver, Object[], Object>>();

        public IDictionary<Type, DependencyEntry> DependencyEntryTable { get; private set; }

        public IDictionary<Tuple<IScopedResolver, DependencyEntry>, Object> ScopedTable { get; private set; }

        public IDictionary<Type, DependencyEntry> PropertyEntryTable { get; private set; }

        public IDictionary<DependencyEntry, bool> HasPropertyEntryTable { get; private set; }

        public DependencyTable(IEnumerable<DependencyEntry> dependencyEntrys)
        {
            if (dependencyEntrys == null) throw new ArgumentNullException(nameof(dependencyEntrys));

            ScopedTable = new ConcurrentDictionary<Tuple<IScopedResolver, DependencyEntry>, Object>();

            DependencyEntryTable = new ConcurrentDictionary<Type, DependencyEntry>(
                dependencyEntrys.
                ToDictionary(entry => entry.ServiceType)
                );

            PropertyEntryTable = new ConcurrentDictionary<Type, DependencyEntry>(
                dependencyEntrys.
                Where(entry => entry.Last.Style.HasFlag(DependencyStyle.PropertyInjection)).
                ToDictionary(entry => entry.ServiceType)
                );

            HasPropertyEntryTable = new ConcurrentDictionary<DependencyEntry, bool>(
                GetAllDependencyEntry(dependencyEntrys).
                Where(entry => HasPropertyDependency(entry)).
                ToDictionary(entry => entry, entry => true)
                );
        }
        
        public void Dispose()
        {
            foreach (var scoped in ScopedTable)
            {
                var disposable = scoped.Value as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
            DependencyEntryTable.Clear();
            PropertyEntryTable.Clear();
            HasPropertyEntryTable.Clear();
            CompileTable.Clear();
            ScopedTable.Clear();
        }

        private IEnumerable<DependencyEntry> GetAllDependencyEntry(IEnumerable<DependencyEntry> source)
        {
            foreach (var item in source)
            {
                for (var entry = item; entry.Next != null; entry = entry.Next)
                {
                    yield return entry;
                }
                yield return item.Last;
            }
        }

        private bool HasPropertyDependency(DependencyEntry entry)
        {
            return entry.GetImplementationType().
                GetProperties(BindingFlags.Public | BindingFlags.Instance).
                Select(p => p.PropertyType).
                Any(p => PropertyEntryTable.ContainsKey(p)
                );
        }

    }
}
