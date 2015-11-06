using FS.DI.Core;
using FS.DI.Resolver;
using System;
using System.Linq;
using System.Reflection;

namespace FS.Extends
{
    public static class ReflectionExtends
    {
        /// <summary>
        /// 返回最佳构造方法
        /// </summary>
        internal static ConstructorInfo GetBastConstructor(this Type type, IDependencyTable dependencyTable)
        {
            var constructors = type.GetConstructors().OrderBy(ctor => ctor.GetParameters().Length).ToArray();
            if (constructors.Length == 0)
            {
                throw new InvalidOperationException(type.FullName + "类没有公共的构造方法。");
            }
            else if (constructors.Length == 1)
            {
                return constructors[0];
            }
            else
            {
                ConstructorInfo bestConstructor = null;
                foreach (var constructor in constructors)
                {
                    if (!constructor.GetParameterTypes().Any(t => !dependencyTable.DependencyEntryTable.ContainsKey(t)))
                    {
                        if (bestConstructor == null)
                        {
                            bestConstructor = constructor;
                        }
                        else
                        {
                            if (bestConstructor.GetParameters().Length == constructor.GetParameters().Length)
                            {
                                throw new InvalidOperationException("类型\"" + type.FullName + "\" 构造方法调用不明确。");
                            }
                            bestConstructor = constructor;
                        }
                    }
                }
                if (bestConstructor == null)
                {
                    throw new InvalidOperationException("类型\"" + type.FullName + "\"未找到合适的构造方法。");
                }
                return bestConstructor;
            }
            throw new InvalidOperationException("类型\"" + type.FullName + "\"未找到合适的构造方法。");
        }

        internal static Object[] GetConstructorParameters(this Type type, IDependencyTable dependencyTable, IDependencyResolver resolver)
        {
            return type.GetBastConstructor(dependencyTable).
                    GetParameters().
                    Select(p => resolver.Resolve(p.ParameterType)).
                    ToArray();
        }    
    }
}
