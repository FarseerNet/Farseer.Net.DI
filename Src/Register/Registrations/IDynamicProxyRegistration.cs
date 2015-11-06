namespace FS.DI.Register
{
    public interface IDynamicProxyRegistration<out TRegistration>
    {
        /// <summary>
        ///     注册为可被动态代理的类型
        /// </summary>
        TRegistration AsDynamicProxy();
    }
}
