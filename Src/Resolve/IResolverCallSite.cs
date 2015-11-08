using FS.DI.Core;
using System;

namespace FS.DI.Resolve
{
    /// <summary>
    /// 解析器调用接口定义
    /// </summary>
    public interface IResolverCallSite
    {
        /// <summary>
        /// 验证解析器上下文
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resolver"></param>
        /// <returns></returns>
        bool Requires(IResolverContext context, IDependencyResolver resolver);
        /// <summary>
        /// 通过上下文调用解析器
        /// </summary>
        /// <param name="context"></param>
        /// <param name="resolver"></param>
        void Resolver(IResolverContext context, IDependencyResolver resolver);
    }
}
 