namespace FS.DI.DynamicProxy
{
    public interface IReturnInvocation : IMethodInvocation
    {
        IReturnParameter Parameter { get; }
    }
}