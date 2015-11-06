using System.Collections;
using System.Collections.Generic;

namespace FS.DI.Resolver
{
    /// <summary>
    /// 解析器集合
    /// </summary>
    internal class CallSiteCollection : ICallSiteCollection
    {
        private readonly IList<IResolverCallSite> _callSiteCollection = new List<IResolverCallSite>();

        public IResolverCallSite this[int index]
        {
            get
            {
                return _callSiteCollection[index];
            }
        }


        /// <summary>
        /// 返回集合中的元素数
        /// </summary>
        public int Count
        {
            get
            {
                return _callSiteCollection.Count;
            }
        }

        /// <summary>
        /// 添加新的解析器
        /// </summary>
        /// <param name="callSite"></param>
        public void Add(IResolverCallSite callSite) => _callSiteCollection.Add(callSite);

        public IEnumerator<IResolverCallSite> GetEnumerator() => _callSiteCollection.GetEnumerator();
        /// <summary>
        /// 移除所有解析器
        /// </summary>
        public void RemoveAll() => _callSiteCollection.Clear();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
