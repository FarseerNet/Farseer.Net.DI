namespace FS.DI.DynamicProxy
{
    public interface IParameterInvocation : IMethodInvocation
    {
        IParameter[] Parameters { get; }
    }
}
