using FS.DI.DynamicProxy;
using System;
using System.Collections.Generic;

namespace FS.Cache
{
    internal class DynamicTypeCacheManager
    {
        private static readonly Dictionary<Type, Type> CacheList = new Dictionary<Type, Type>();

        private static readonly Object _sync = new Object();

        private readonly Type _key;
    
        public DynamicTypeCacheManager(Type key)
        {
            _key = key;
        }

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

                return (CacheList[_key] = DynamicTypeProvider.Current.CreateDynamicType(_key));
            }
        }

        public static Type Cache(Type key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));
            return new DynamicTypeCacheManager(key).GetValue();
        }
    }
}
