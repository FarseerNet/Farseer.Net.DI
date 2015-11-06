namespace FS.DI.Register
{
    public interface IScopedRegistration<out TRegistration> 
    {
        /// <summary>
        ///     注册为作用域的生命周期
        /// </summary>
        /// <returns></returns>
        TRegistration AsScopedLifetime();
    }
}
