namespace FS.DI.DynamicProxy
{
    /// <summary>
    ///     参数代理调用
    /// </summary>
    public interface IParameterInvocation : IMethodInvocation
    {
        /// <summary>
        ///     方法参数
        /// </summary>
        IParameter[] Parameters { get; }
    }
}
