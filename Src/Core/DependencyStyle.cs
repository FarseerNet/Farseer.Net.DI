using System;

namespace FS.DI
{
    [Flags]
    internal enum DependencyStyle
    {
        /// <summary>
        ///     默认类型
        /// </summary>
        Default = 1,

        /// <summary>
        ///     属性注入
        /// </summary>
        PropertyInjection = 2,

        /// <summary>
        ///     动态代理
        /// </summary>
        DynamicProxy = 4,
    }
}