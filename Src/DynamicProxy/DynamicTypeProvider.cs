﻿using FS.Extends;
using System;

namespace FS.DI.DynamicProxy
{
    public class DynamicTypeProvider
    {
        private static readonly DynamicTypeProvider Instance = new DynamicTypeProvider();

        private IDynamicTypeProvider _current;

        private DynamicTypeProvider()
        {
            this.InnerSetProvider(new DefaultDynamicTypeProvider());
        }

        /// <summary>
        ///     当前的IDynamicTypeProvider实例
        /// </summary>
        public static IDynamicTypeProvider Current => Instance._current;

        /// <summary>
        ///     设置IDynamicTypeProvider的实例
        /// </summary>
        public static void SetProvider(IDynamicTypeProvider dynamicTypeProvider)
        {
            Instance.InnerSetProvider(dynamicTypeProvider);
        }

        /// <summary>
        ///     设置自定义的动态类型创建
        /// </summary>
        public static void SetProvider(Func<Type, Type> dynamicTypeProvider)
        {
            Instance.InnerSetProvider(dynamicTypeProvider);
        }

        /// <summary>
        ///     设置IDynamicTypeProvider的实例
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private void InnerSetProvider(IDynamicTypeProvider dynamicTypeProvider)
        {
            if (dynamicTypeProvider == null)
            {
                throw new ArgumentNullException(nameof(dynamicTypeProvider));
            }
            _current = dynamicTypeProvider;
        }

        /// <summary>
        ///     设置自定义的动态类型创建
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        private void InnerSetProvider(Func<Type, Type> dynamicTypeProvider)
        {
            if (dynamicTypeProvider == null)
            {
                throw new ArgumentNullException(nameof(dynamicTypeProvider));
            }
            _current = new DelegateDynamicTypeProvider(dynamicTypeProvider);
        }

        /// <summary>
        ///     默认动态类型创建器
        /// </summary>
        private class DefaultDynamicTypeProvider : IDynamicTypeProvider
        {
            /// <summary>
            ///     创建动态类型
            /// </summary>
            /// <exception cref="MissingMethodException"></exception>
            public Type CreateType(Type parentType)
            {
                try
                {
                    if (parentType.IsInterface)
                        throw new MissingMethodException("无法创建接口的实例。");
                    if (parentType.IsAbstract)
                        throw new MissingMethodException("无法创建抽象类的实例。");

                    var builder = DynamicModule.Current.Module.
                        DefineProxyType(parentType).DefineConstructors(parentType).DefineOverrideMethods(parentType);

                    parentType.GetInterfaces().ForEach(interfaceType =>
                        builder.DefineExplicitInterfaceMethods(interfaceType, parentType));

                    return builder.CreateType();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"类型\"{parentType.FullName}\"生成代理类时发生异常：{ex.Message}", ex);
                }  
            }
        }

        /// <summary>
        ///     委托动态类型创建器
        /// </summary>
        private class DelegateDynamicTypeProvider : IDynamicTypeProvider
        {
            private readonly Func<Type, Type> _creator;

            public DelegateDynamicTypeProvider(Func<Type, Type> creator)
            {
                _creator = creator;
            }

            public Type CreateType(Type parentType)
            {
                if (parentType == null) throw new ArgumentNullException(nameof(parentType));
                return _creator(parentType);
            }
        }
    }
}
