namespace FS.DI.Register
{
    public interface ITransientRegistration<out TRegistration>
    {
        /// <summary>
        /// 注册为瞬态实例的生命周期
        /// </summary>
        TRegistration AsTransientLifetime();
    }
}
