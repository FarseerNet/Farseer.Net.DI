namespace FS.DI.Registration
{
    /// <summary>
    ///     依赖服务属性注入注册
    /// </summary>
    public interface IPropertyRegistration<out TRegistration>
    {
        /// <summary>
        ///     作为自动属性注入的依赖服务
        /// </summary>
        TRegistration AsPropertyInjection();
    }
}
