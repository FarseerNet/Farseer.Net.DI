namespace FS.DI.Registration
{
    /// <summary>
    ///     依赖服务动态代理注册
    /// </summary>
    public interface IDynamicProxyRegistration<out TRegistration>
    {
        /// <summary>
        ///     作为动态代理实现的依赖服务
        /// </summary>
        TRegistration AsDynamicProxy();
    }
}
