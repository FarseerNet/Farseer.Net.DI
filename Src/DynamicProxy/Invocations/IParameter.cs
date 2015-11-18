using System;
using System.Reflection;

namespace FS.DI.DynamicProxy
{
    /// <summary>
    ///     参数
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        ///     参数值
        /// </summary>
        Object Value { get; set; }
        /// <summary>
        ///     参数名称
        /// </summary>
        string Name { get; }
        /// <summary>
        ///     参数元数据
        /// </summary>
        ParameterInfo ParameterInfo { get; }
    }
}
