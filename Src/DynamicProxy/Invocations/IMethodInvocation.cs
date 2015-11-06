using System.Reflection;

namespace FS.DI.DynamicProxy
{
    public interface IMethodInvocation : IInvocation
    {
        /// <summary>
        ///     获取被拦截的方法
        /// </summary>
        MethodInfo InterceptedMethod { get; }

        /// <summary>
        ///     获取或设置一个值，该值指示是否已处理方法拦截，
        ///     值为true时将不执行后续方法拦截器和被拦截的方法，
        ///     该值仅对方法执行前的拦截器有效。
        /// </summary>
        bool ExecutedHandled { get; set; }
    }
}
