using System;

namespace FS.DI.DynamicProxy
{
    /// <summary>
    /// 动态类型创建器
    /// </summary>
    public interface IDynamicTypeProvider
    {
        /// <summary>
        ///     创建动态类型
        /// </summary>
        Type CreateType(Type parentType);
    }
}
