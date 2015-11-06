using FS.Common;
using FS.Extends;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace FS.DI.DynamicProxy
{
    /// <summary>
    ///     程序集内部动态类型
    /// </summary>
    internal class InternalDynamicTypeProvider : IDynamicTypeProvider
    {
        private static ConcurrentDictionary<Type, Type> _dynamicTypeMap = new ConcurrentDictionary<Type, Type>();

        /// <summary>
        ///     创建动态类型
        /// </summary>
        public Type CreateDynamicType(Type parentType)
        {
            return _dynamicTypeMap.GetOrAdd(parentType, _ =>
            {
                if (_dynamicTypeMap.ContainsKey(parentType)) return _dynamicTypeMap[parentType];
                var name = "_" + (parentType.IsInterface ? parentType.Name.Substring(1) : parentType.Name);
                var builder = DynamicHelper.ModuleBuilder.DefineType(name, TypeAttributes.NotPublic,
                    parentType.IsClass ? parentType : typeof(Object),
                    parentType.IsClass ? parentType.GetInterfaces() : new Type[] { parentType }.Concat(parentType.GetInterfaces()).ToArray());
                parentType.GetProperties().Where(p => p.GetGetMethod().IsAbstract).ForEach(property => DynamicHelper.DefineProperty(builder, property));
                if (parentType.IsInterface)
                {
                    parentType.GetInterfaces().ForEach(type => type.GetProperties().Where(p => p.GetGetMethod().IsAbstract).ForEach(property => DynamicHelper.DefineProperty(builder, property)));
                }
                return builder.CreateType();
            });
        }

        /// <summary>
        ///     创建类型
        /// </summary>
        public static Type CreateType(Type parentType)
        {
            return new InternalDynamicTypeProvider().CreateDynamicType(parentType);
        }

        /// <summary>
        ///     创建类型
        /// </summary>
        public static Type CreateType<T>()
        {
            return CreateType(typeof(T));
        }
    }
}
