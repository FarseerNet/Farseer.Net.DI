using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FS.DI.DynamicProxy
{
    /// <summary>
    ///     动态程序集
    /// </summary>
    internal abstract class DynamicAssembly
    {
        private static readonly DynamicAssembly _current = new RuntimeDynamicAssembly();

        /// <summary>
        ///     获取当前动态程序集的实例
        /// </summary>
        public static DynamicAssembly Current { get; } = _current;

        /// <summary>
        ///     获取动态程序集名称
        /// </summary>
        public abstract AssemblyName AssemblyName { get; }

        /// <summary>
        ///     获取动态程序集
        /// </summary>
        public abstract AssemblyBuilder AssemblyBuilder { get; }

        /// <summary>
        ///     获取动态程序集中的模块
        /// </summary>
        public abstract ModuleBuilder ModuleBuilder { get; }

        /// <summary>
        ///     运行时动态程序集
        /// </summary>
        private sealed class RuntimeDynamicAssembly : DynamicAssembly
        {
            private AssemblyName _assemblyName;
            private AssemblyBuilder _assemblyBuilder;
            private ModuleBuilder _moduleBuilder;

            public RuntimeDynamicAssembly()
            {
                _assemblyName = new AssemblyName("Farseer.Net.DI._Dynamic");
                _assemblyBuilder= AppDomain.CurrentDomain.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.Run);
                _moduleBuilder = _assemblyBuilder.DefineDynamicModule(AssemblyName.Name);
            }

            /// <summary>
            ///     获取动态程序集
            /// </summary>
            public override AssemblyBuilder AssemblyBuilder
            {
                get
                {
                    return _assemblyBuilder;
                }
            }

            /// <summary>
            ///     获取动态程序集名称
            /// </summary>
            public override AssemblyName AssemblyName
            {
                get
                {
                    return _assemblyName;
                }
            }

            /// <summary>
            ///     获取动态程序集中的模块
            /// </summary>
            public override ModuleBuilder ModuleBuilder
            {
                get
                {
                    return _moduleBuilder;
                }
            }
        }
    }
}
