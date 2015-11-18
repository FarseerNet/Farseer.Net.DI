namespace FS.DI.DynamicProxy
{
    /// <summary>
    ///     返回代理调用
    /// </summary>
    public interface IReturnInvocation : IMethodInvocation
    {
        /// <summary>
        ///     返回值参数
        /// </summary>
        IReturnParameter Parameter { get; }
    }
}