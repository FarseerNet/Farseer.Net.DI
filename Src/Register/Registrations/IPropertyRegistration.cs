namespace FS.DI.Register
{
    public interface IPropertyRegistration<out TRegistration>
    {
        /// <summary>
        ///     注册为自动注入的属性
        /// </summary>
        TRegistration AsPropertyInjection();
    }
}
