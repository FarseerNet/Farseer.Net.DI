namespace FS.DI.Register
{
    public interface ISingletonRegistration<out TRegistration> : IPropertyRegistration<TRegistration>
    {
        /// <summary>
        /// 注册为单例的生命周期
        /// </summary>
        /// <returns></returns>
        TRegistration AsSingletonLifetime();
    }
    public interface ISingletonRegistration: ISingletonRegistration<ISingletonRegistration>
    {
    }
}
