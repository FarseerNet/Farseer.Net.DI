using FS.DI.DynamicProxy;
using System;
using System.Collections.Generic;

namespace FS.Cache
{
    /// <summary>
    ///     动态类型缓存管理
    /// </summary>
    internal sealed class DynamicTypeCacheManager
    {
        /// <summary>
        ///     缓存字典
        /// </summary>
        private static readonly Dictionary<Type, Type> CacheList = new Dictionary<Type, Type>();

        /// <summary>
        ///     线程锁
        /// </summary>
        private static readonly Object _sync = new Object();
        /// <summary>
        ///     缓存Key
        /// </summary>
        private readonly Type _key;
    
        public DynamicTypeCacheManager(Type key)
        {
            _key = key;
        }

        /// <summary>
        ///     获取缓存值
        /// </summary>
        public Type GetValue()
        {
            if (CacheList.ContainsKey(_key)) return CacheList[_key];
            return SetCacheLock();
        }

        private Type SetCacheLock()
        {
            lock (_sync)
            {
                if (CacheList.ContainsKey(_key)) return CacheList[_key];

                return (CacheList[_key] = DynamicTypeProvider.Current.CreateType(_key));
            }
        }

        /// <summary>
        ///     获取缓存
        /// </summary>
        public static Type GetCache(Type key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return new DynamicTypeCacheManager(key).GetValue();
        }
    }
}
