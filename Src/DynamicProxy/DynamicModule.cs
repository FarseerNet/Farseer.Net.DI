using System;
using System.Reflection;
using System.Reflection.Emit;

namespace FS.DI.DynamicProxy
{
    /// <summary>
    ///     动态程序集
    /// </summary>
    internal abstract class DynamicModule
    {
        private static readonly DynamicModule _current = new RuntimeDynamicModule();

        /// <summary>
        ///     获取当前动态程序集的实例
        /// </summary>
        public static DynamicModule Current => _current;

        /// <summary>
        ///     获取动态程序集名称
        /// </summary>
        public abstract AssemblyName AssemblyName { get; }

        /// <summary>
        ///     获取动态程序集
        /// </summary>
        public abstract AssemblyBuilder Assembly { get; }

        /// <summary>
        ///     获取动态程序集中的模块
        /// </summary>
        public abstract ModuleBuilder Module { get; }

        /// <summary>
        ///     运行时动态程序集
        /// </summary>
        private sealed class RuntimeDynamicModule : DynamicModule
        {
            private readonly string DEFAULT_MODULE_NAME = "Farseer.Net.DI._Dynamic";
            private readonly AssemblyName _assemblyName;
            private readonly AssemblyBuilder _assemblyBuilder;
            private readonly ModuleBuilder _moduleBuilder;

            public RuntimeDynamicModule()
            {
                _assemblyName = new AssemblyName(DEFAULT_MODULE_NAME);
                _assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(_assemblyName,
                    AssemblyBuilderAccess.RunAndSave);
                _moduleBuilder = _assemblyBuilder.DefineDynamicModule(DEFAULT_MODULE_NAME, DEFAULT_MODULE_NAME + ".dll");
            }

            /// <summary>
            ///     获取动态程序集
            /// </summary>
            public override AssemblyBuilder Assembly => _assemblyBuilder;

            /// <summary>
            ///     获取动态程序集名称
            /// </summary>
            public override AssemblyName AssemblyName => _assemblyName;

            /// <summary>
            ///     获取动态程序集中的模块
            /// </summary>
            public override ModuleBuilder Module => _moduleBuilder;
        }
    }
}
