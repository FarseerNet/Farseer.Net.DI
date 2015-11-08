using System.Collections;
using System.Collections.Generic;

namespace FS.DI.Resolve
{
    /// <summary>
    /// 解析器集合
    /// </summary>
    public interface ICallSiteCollection : IEnumerable<IResolverCallSite>, IEnumerable
    {

        IResolverCallSite this[int index] { get;}
        /// <summary>
        /// 返回集合中的元素数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// 添加新的解析器
        /// </summary>
        /// <param name="callSite"></param>
        void Add(IResolverCallSite callSite);

        /// <summary>
        /// 移除所有解析器
        /// </summary>
        void RemoveAll();
    }
}
