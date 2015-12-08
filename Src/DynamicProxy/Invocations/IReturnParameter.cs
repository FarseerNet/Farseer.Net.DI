using System;

namespace FS.DI.DynamicProxy
{
    /// <summary>
    ///     返回参数
    /// </summary>
    public interface IReturnParameter
    {
        /// <summary>
        ///     返回值
        /// </summary>
        object Value { get; set; }

        /// <summary>
        ///     返回值类型
        /// </summary>
        Type ReturnType { get; }
    }
}
