using FS.DI.DynamicProxy;
using FS.Extends;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace FS.Common
{
    internal static class DynamicHelper
    {
        /// <summary>
        ///     动态程序集名称
        /// </summary>
        internal static AssemblyName AssemblyName { get; } = new AssemblyName("Farseer.Net.DI._Dynamic");

        /// <summary>
        ///     动态程序集
        /// </summary>
        internal static AssemblyBuilder AssemblyBuilder { get; } = AppDomain.CurrentDomain.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run);

        /// <summary>
        ///     动态模块
        /// </summary>
        //internal static ModuleBuilder ModuleBuilder { get; } = AssemblyBuilder.DefineDynamicModule(AssemblyName.Name, AssemblyName.Name + ".dll");
        internal static ModuleBuilder ModuleBuilder { get; } = AssemblyBuilder.DefineDynamicModule(AssemblyName.Name);


        /// <summary>
        ///     返回动态类型的name
        /// </summary>
        internal static String DynamicTypeName(this Type parentType) => "_Dynamic" + parentType.Name;

        /// <summary>
        ///     构造指定基类的TypeBuilder
        /// </summary>
        internal static TypeBuilder DefineProxyType(this ModuleBuilder moduleBuilder, Type parentType)
        {
            if (parentType == null) throw new ArgumentNullException(nameof(parentType));
            if (parentType.IsInterface || parentType.IsAbstract)
                throw new InvalidOperationException("接口或抽象类\"" + parentType.FullName + "\"不能生成代理类。");
            if (parentType.IsNotPublic)
                throw new InvalidOperationException("非公共类型\"" + parentType.FullName + "\"不能生成代理类。");
            if (parentType.IsSealed)
                throw new InvalidOperationException("封闭类型\"" + parentType.FullName + "\"不能生成代理类。");

            return moduleBuilder.DefineType(parentType.DynamicTypeName(), parentType.Attributes, parentType, parentType.GetInterfaces());
        }       

        /// <summary>
        ///     根据指定基类的构造方法，向类型中添加构造方法
        /// </summary>
        internal static TypeBuilder DefineConstructors(this TypeBuilder typeBuilder, Type parentType)
        {
            parentType.GetConstructors().ForEach(constructor => typeBuilder.DefineConstructor(constructor.Attributes, constructor.CallingConvention, constructor.GetParameterTypes().ToArray()).GetILGenerator().CallBaseConstructor(constructor).Return());
            return typeBuilder;
        }

        /// <summary>
        ///     根据指定基类，向类型中添加重写的方法
        /// </summary>
        internal static TypeBuilder DefineOverrideMethods(this TypeBuilder typeBuilder, Type parentType)
        {
            var methods = parentType.GetProxyMethods();
            var get_InterceptedMethod = typeof(InterceptorExtends).GetMethod("GetInterceptedMethod", Type.EmptyTypes);
            var get_InterceptorsMethod = typeof(InterceptorExtends).GetMethod("GetInterceptors");
            var get_MakeGenericMethod = typeof(InterceptorExtends).GetMethod("MakeGenericMethod");
            foreach (var method in methods)
            {
                var interceptors = method.GetInterceptors(method.DeclaringType);
                if (interceptors.Any())
                {
                    var methodBuilder = typeBuilder.DefineOverrideMethod(method);
                    var ilGenerator = methodBuilder.GetILGenerator();
                    var returnValue = method.ReturnType != typeof(void);

                    var returnLocal = default(LocalBuilder);
                    var interceptedMethodLocal = ilGenerator.DeclareLocal(typeof(MethodInfo));
                    var interceptorsLocal = ilGenerator.DeclareLocal(typeof(IInterceptor[]));
                    var interceptedTypeLocal = ilGenerator.DeclareLocal(parentType);
                    if (returnValue)
                        returnLocal = ilGenerator.DeclareLocal(method.ReturnType);
                    ilGenerator.GetBaseType().StoreLocal(interceptedTypeLocal).Call(get_InterceptedMethod);
                    if (method.IsGenericMethod) ilGenerator.Call(get_MakeGenericMethod);
                    ilGenerator.StoreLocal(interceptedMethodLocal).LoadLocal(interceptedMethodLocal).LoadLocal(interceptedTypeLocal).Call(get_InterceptorsMethod).StoreLocal(interceptorsLocal);
                    MakeGenericMethod(ilGenerator, method, interceptedMethodLocal);

                    ilGenerator.Try(il =>
                    {
                        ParameterIntercept(ilGenerator, interceptors, new LocalBuilder[] { interceptorsLocal, interceptedMethodLocal, interceptedTypeLocal, returnLocal }, method);
                        MethodIntercept(ilGenerator, interceptors,
                           new LocalBuilder[] { interceptorsLocal, interceptedMethodLocal, interceptedTypeLocal, returnLocal }, new bool[] { returnValue },
                           _ =>
                           {
                               //ilGenerator.Try(il =>
                               //{
                               ilGenerator.CallBase(method);
                               if (returnValue) ilGenerator.StoreLocal(returnLocal);
                               //}).
                               //Catch(il => ExcepionIntercept(il, interceptors, new LocalBuilder[] { interceptorsLocal, interceptedMethodLocal, interceptedTypeLocal })).EndException();
                           });
                        ReturnIntercept(ilGenerator, interceptors, new LocalBuilder[] { interceptorsLocal, interceptedMethodLocal, interceptedTypeLocal, returnLocal }, new bool[] { returnValue });
                    }).
                    Catch(il => ExcepionIntercept(il, interceptors, new LocalBuilder[] { interceptorsLocal, interceptedMethodLocal, interceptedTypeLocal })).EndException();
                    if (returnValue) ilGenerator.LoadLocal(returnLocal);
                    ilGenerator.Return();
                }
            }
            return typeBuilder;
        }

        /// <summary>
        ///     根据指定接口，向类型中添加显示实现的方法
        /// </summary>
        internal static TypeBuilder DefineExplicitInterfaceMethods(this TypeBuilder typeBuilder, Type interfaceType, Type parentType)
        {
            var methods = interfaceType.GetProxyMethods();
            var get_InterfaceMethod = typeof(Type).GetMethod("GetInterface", new Type[] { typeof(String) });
            var get_InterceptorsMethod = typeof(InterceptorExtends).GetMethod("GetInterceptors");
            var get_Intercepted_IMethod = typeof(InterceptorExtends).GetMethod("GetIntercepted_IMethod", new Type[] { typeof(String) });
            foreach (var method in methods)
            {
                var interceptors = method.GetInterceptors(method.DeclaringType);
                if (interceptors.Any())
                {
                    var methodBuilder = typeBuilder.DefineExplicitInterfaceMethod(method);

                    var ilGenerator = methodBuilder.GetILGenerator();
                    var returnValue = method.ReturnType != typeof(void);
                    var returnLocal = default(LocalBuilder);
                    var interceptedMethodLocal = ilGenerator.DeclareLocal(typeof(MethodInfo));
                    var interceptorsLocal = ilGenerator.DeclareLocal(typeof(IInterceptor[]));
                    var interceptedTypeLocal = ilGenerator.DeclareLocal(interfaceType);
                    if (returnValue)
                        returnLocal = ilGenerator.DeclareLocal(method.ReturnType);
                    ilGenerator.LoadString(interfaceType.Name).Call(get_Intercepted_IMethod);
                    ilGenerator.StoreLocal(interceptedMethodLocal).GetThisType().LoadString(interfaceType.Name).Callvirt(get_InterfaceMethod).StoreLocal(interceptedTypeLocal);
                    ilGenerator.LoadLocal(interceptedMethodLocal).LoadLocal(interceptedTypeLocal).Call(get_InterceptorsMethod).StoreLocal(interceptorsLocal);
                    MakeGenericMethod(ilGenerator, method, interceptedMethodLocal);
                    ParameterIntercept(ilGenerator, interceptors, new LocalBuilder[] { interceptorsLocal, interceptedMethodLocal, interceptedTypeLocal, returnLocal }, method);
                    MethodIntercept(ilGenerator, interceptors,
                        new LocalBuilder[] { interceptorsLocal, interceptedMethodLocal, interceptedTypeLocal, returnLocal }, new bool[] { returnValue },
                        _ =>
                        {
                            ilGenerator.Try(il =>
                            {
                                var baseMethod = parentType.GetMethods().First(m => m.EqualMethod(method));
                                if (baseMethod == null)
                                {
                                    throw new NotImplementedException(parentType.FullName + " 未实现方法 " + method.ToString());
                                }
                                ilGenerator.CallBase(baseMethod);
                                if (returnValue) ilGenerator.StoreLocal(returnLocal);
                            }).
                            Catch(il => ExcepionIntercept(il, interceptors, new LocalBuilder[] { interceptorsLocal, interceptedMethodLocal, interceptedTypeLocal })).EndException();
                        });
                    ReturnIntercept(ilGenerator, interceptors, new LocalBuilder[] { interceptorsLocal, interceptedMethodLocal, interceptedTypeLocal, returnLocal }, new bool[] { returnValue });
                    if (returnValue) ilGenerator.LoadLocal(returnLocal);
                    ilGenerator.Return();
                }
            }
            return typeBuilder;
        }

        /// <summary>
        ///     方法参数拦截
        /// </summary>>
        private static void ParameterIntercept(ILGenerator ilGenerator, IInterceptor[] interceptors, LocalBuilder[] local, MethodInfo method)
        {
            var parameterInterceptors = interceptors.GetParameterInterceptors();
            if (parameterInterceptors.Any())
            {
                var invocationType = InternalDynamicTypeProvider.CreateType<IParameterInvocation>();
                var parameterType = InternalDynamicTypeProvider.CreateType<IParameter>();
                var set_InterceptedMethod = invocationType.GetMethod("set_InterceptedMethod");
                var set_InterceptedType = invocationType.GetMethod("set_InterceptedType");
                var set_InterceptedInstance = invocationType.GetMethod("set_InterceptedInstance");
                var set_Parameters = invocationType.GetMethod("set_Parameters");
                var set_Value = parameterType.GetMethod("set_Value");
                var set_Name = parameterType.GetMethod("set_Name");
                var set_ParameterInfo = parameterType.GetMethod("set_ParameterInfo");
                var get_Name = typeof(ParameterInfo).GetMethod("get_Name");
                var get_ParameterInterceptors = typeof(InterceptorExtends).GetMethod("GetParameterInterceptors");
                var get_CurrentParameter = typeof(InterceptorExtends).GetMethod("GetCurrentParameter");
                var on_Intercept = typeof(IParameterInterceptor).GetMethod("OnParameterExecuting");
                var interceptorLocal = ilGenerator.DeclareLocal(typeof(IParameterInterceptor[]));
                var parametersLocal = ilGenerator.DeclareLocal(typeof(IParameter[]));
                //ilGenerator.Try(il =>
                //{
                ilGenerator.LoadLocal(local[0]).Call(get_ParameterInterceptors).StoreLocal(interceptorLocal);
                var invocationLocal = ilGenerator.DeclareLocal(invocationType);
                ilGenerator.New(invocationType.GetConstructor(Type.EmptyTypes)).StoreLocal(invocationLocal);
                ilGenerator.LoadLocal(invocationLocal).LoadLocal(local[1]).Callvirt(set_InterceptedMethod);
                ilGenerator.LoadLocal(invocationLocal).LoadLocal(local[2]).Callvirt(set_InterceptedType);
                ilGenerator.LoadLocal(invocationLocal).This().Callvirt(set_InterceptedInstance);
                ilGenerator.ForEach(parameterInterceptors, (_, interceptor, i) =>
                      {
                          var parameters = method.GetParameters().Where(p => p.GetCustomAttributes(false).Any(c => c.GetType() == interceptor.GetType())).ToArray();
                          ilGenerator.NewArray(typeof(IParameter), parameters.Length).StoreLocal(parametersLocal);
                          ilGenerator.ForEach(parameters, (__, parameter, j) =>
                              ilGenerator.ForEach(method.GetParameters(), (___, arg, k) =>
                              {
                                  if (arg == parameter)
                                  {
                                      var argLocal = ilGenerator.DeclareLocal(parameterType);
                                      ilGenerator.New(parameterType.GetConstructor(Type.EmptyTypes)).StoreLocal(argLocal);
                                      ilGenerator.LoadLocal(argLocal).LoadArgument(k + 1).Box(arg.ParameterType).Callvirt(set_Value);
                                      ilGenerator.LoadLocal(argLocal).LoadLocal(local[1]).LoadInt(k).Call(get_CurrentParameter).Callvirt(set_ParameterInfo);
                                      ilGenerator.LoadLocal(argLocal).LoadLocal(local[1]).LoadInt(k).Call(get_CurrentParameter).Callvirt(get_Name).Callvirt(set_Name);
                                      ilGenerator.LoadLocal(parametersLocal).LoadInt(j).LoadLocal(argLocal).SetArrayItemRef();
                                  }
                              }));
                          ilGenerator.LoadLocal(invocationLocal).LoadLocal(parametersLocal).Callvirt(set_Parameters);
                          ilGenerator.LoadLocal(interceptorLocal).LoadArrayItem(i).LoadLocal(invocationLocal).Callvirt(on_Intercept);
                      });
                //}).
                //Catch(il => ExcepionIntercept(il, interceptors, local)).EndException();
            }
        }

        /// <summary>
        ///     异常拦截
        /// </summary>
        private static void ExcepionIntercept(ILGenerator ilGenerator, IInterceptor[] interceptors, LocalBuilder[] local)
        {
            var excepionInterceptors = interceptors.GetExcepionInterceptors();
            if (excepionInterceptors.Any())
            {
                var invocationType = InternalDynamicTypeProvider.CreateType<IExceptionInvocation>();
                var set_Exception = invocationType.GetMethod("set_Exception");
                var set_InterceptedType = invocationType.GetMethod("set_InterceptedType");
                var set_InterceptedInstance = invocationType.GetMethod("set_InterceptedInstance");
                var get_ExceptionHandled = invocationType.GetMethod("get_ExceptionHandled");
                var get_ExcepionInterceptMethod = typeof(IExceptionInterceptor).GetMethod("OnExcepion");
                var get_ExcepionInterceptorsMethod = typeof(InterceptorExtends).GetMethod("GetExcepionInterceptors");
                var exceptionLocal = ilGenerator.DeclareLocal(typeof(Exception));
                var interceptorLocal = ilGenerator.DeclareLocal(typeof(IExceptionInterceptor[]));
                var invocationLocal = ilGenerator.DeclareLocal(invocationType);
                var breakExceptionLable = ilGenerator.DefineLabel();
                ilGenerator.StoreLocal(exceptionLocal).LoadLocal(local[0]).
                    Call(get_ExcepionInterceptorsMethod).StoreLocal(interceptorLocal);
                ilGenerator.New(invocationType.GetConstructor(Type.EmptyTypes)).StoreLocal(invocationLocal);
                ilGenerator.LoadLocal(invocationLocal).LoadLocal(exceptionLocal).Callvirt(set_Exception);
                ilGenerator.LoadLocal(invocationLocal).LoadLocal(local[2]).Callvirt(set_InterceptedType);
                ilGenerator.LoadLocal(invocationLocal).This().Callvirt(set_InterceptedInstance);
                ilGenerator.ForEach(excepionInterceptors, (il, interceptor, index) =>
                    il.LoadLocal(invocationLocal).Callvirt(get_ExceptionHandled).
                    False(breakExceptionLable).
                    LoadLocal(interceptorLocal).LoadArrayItem(index).LoadLocal(invocationLocal).Callvirt(get_ExcepionInterceptMethod)).
                    MarkLabelFor(breakExceptionLable); 
            }
            else
            {
                ilGenerator.Emit(OpCodes.Throw);
            }
        }

        /// <summary>
        ///     方法拦截
        /// </summary>
        private static void MethodIntercept(ILGenerator ilGenerator, IInterceptor[] interceptors, LocalBuilder[] local, bool[] boolean, Action<ILGenerator> method)
        {
            var methodInterceptors = interceptors.GetMethodInterceptor();
            if (methodInterceptors.Any())
            {
                var invocationType = InternalDynamicTypeProvider.CreateType<IMethodInvocation>();
                var set_InterceptedMethod = invocationType.GetMethod("set_InterceptedMethod");
                var set_InterceptedType = invocationType.GetMethod("set_InterceptedType");
                var set_InterceptedInstance = invocationType.GetMethod("set_InterceptedInstance");
                var set_ExecutedHandled = invocationType.GetMethod("set_ExecutedHandled");
                var get_ExecutedHandled = invocationType.GetMethod("get_ExecutedHandled");
                var get_GetMethodInterceptor = typeof(InterceptorExtends).GetMethod("GetMethodInterceptor");
                var get_GetExecutedMethodInterceptor = typeof(InterceptorExtends).GetMethod("GetExecutedMethodInterceptor");
                var On_MethodExecuting = typeof(IMethodInterceptor).GetMethod("OnMethodExecuting");
                var On_MethodExecuted = typeof(IMethodInterceptor).GetMethod("OnMethodExecuted");
                var interceptorLocal = ilGenerator.DeclareLocal(typeof(IMethodInterceptor[]));
                var executedInterceptorLocal = ilGenerator.DeclareLocal(typeof(IMethodInterceptor[]));
                var invocationLocal = ilGenerator.DeclareLocal(invocationType);
                var endLable = ilGenerator.DefineLabel();
                //ilGenerator.Try(il =>
                //{
                ilGenerator.LoadLocal(local[0]).
                        Call(get_GetMethodInterceptor).StoreLocal(interceptorLocal);
                ilGenerator.LoadLocal(local[0]).
                        Call(get_GetExecutedMethodInterceptor).StoreLocal(executedInterceptorLocal);
                ilGenerator.New(invocationType.GetConstructor(Type.EmptyTypes)).StoreLocal(invocationLocal);
                ilGenerator.LoadLocal(invocationLocal).LoadLocal(local[1]).Callvirt(set_InterceptedMethod);
                ilGenerator.LoadLocal(invocationLocal).LoadLocal(local[2]).Callvirt(set_InterceptedType);
                ilGenerator.LoadLocal(invocationLocal).This().Callvirt(set_InterceptedInstance);
                ilGenerator.ForEach(methodInterceptors, (_, interceptor, index) =>
                     _.LoadLocal(invocationLocal).Callvirt(get_ExecutedHandled).
                        False(endLable).LoadLocal(interceptorLocal).LoadArrayItem(index).LoadLocal(invocationLocal).Callvirt(On_MethodExecuting));
                method(ilGenerator);
                ilGenerator.ForEach(methodInterceptors.OrderBy(i => i.ExecutedOrder), (_, interceptor, index) =>
                   _.LoadLocal(invocationLocal).Callvirt(get_ExecutedHandled).
                      False(endLable).LoadLocal(executedInterceptorLocal).LoadArrayItem(index).LoadLocal(invocationLocal).Callvirt(On_MethodExecuted));
                ilGenerator.MarkLabelFor(endLable);
                //}).
                //Catch(il => ExcepionIntercept(il, interceptors, local)).EndException();
            }
            else
            {
                method(ilGenerator);
            }
        }

        /// <summary>
        ///     方法返回拦截
        /// </summary>
        private static void ReturnIntercept(ILGenerator ilGenerator, IInterceptor[] interceptors,
            LocalBuilder[] local /* interceptorsLocal,interceptedMethodLocal,interceptedTypeLocal */,
            bool[] boolean /*hasValue,*/)
        {
            var returnValueInvocations = interceptors.GetReturnValueInterceptors();
            if (returnValueInvocations.Any())
            {
                var invocationType = InternalDynamicTypeProvider.CreateType<IReturnInvocation>();
                var parameterType = InternalDynamicTypeProvider.CreateType<IReturnParameter>();
                var set_InterceptedMethod = invocationType.GetMethod("set_InterceptedMethod");
                var set_InterceptedType = invocationType.GetMethod("set_InterceptedType");
                var set_InterceptedInstance = invocationType.GetMethod("set_InterceptedInstance");
                var set_Parameter = invocationType.GetMethod("set_Parameter");
                var get_Parameter = invocationType.GetMethod("get_Parameter");
                var get_Value = parameterType.GetMethod("get_Value");
                var set_ReturnType = parameterType.GetMethod("set_ReturnType");
                var set_Value = parameterType.GetMethod("set_Value");
                var get_ReturnType = typeof(MethodInfo).GetMethod("get_ReturnType");
                var on_Intercept = typeof(IReturnInterceptor).GetMethod("OnReturnExecuted");
                var get_ReturnInterceptorsMethod = typeof(InterceptorExtends).GetMethod("GetReturnValueInterceptors");
                var interceptorLocal = ilGenerator.DeclareLocal(typeof(IReturnInterceptor[]));
                var invocationLocal = ilGenerator.DeclareLocal(invocationType);
                var parameterLocal = ilGenerator.DeclareLocal(parameterType);
                //ilGenerator.Try(il =>
                //{                
                ilGenerator.LoadLocal(local[0]).Call(get_ReturnInterceptorsMethod).StoreLocal(interceptorLocal);
                ilGenerator.New(invocationType.GetConstructor(Type.EmptyTypes)).StoreLocal(invocationLocal);
                ilGenerator.LoadLocal(invocationLocal).LoadLocal(local[1]).Callvirt(set_InterceptedMethod);
                ilGenerator.LoadLocal(invocationLocal).LoadLocal(local[2]).Callvirt(set_InterceptedType);
                ilGenerator.LoadLocal(invocationLocal).This().Callvirt(set_InterceptedInstance);
                ilGenerator.New(parameterType.GetConstructor(Type.EmptyTypes)).StoreLocal(parameterLocal);
                if (boolean[0])
                    ilGenerator.LoadLocal(parameterLocal).LoadLocal(local[1]).Callvirt(get_ReturnType).Callvirt(set_ReturnType).
                            LoadLocal(invocationLocal).LoadLocal(local[3]).Callvirt(set_Value);
                else
                    ilGenerator.LoadLocal(parameterLocal).Typeof(typeof(void)).Callvirt(set_ReturnType);
                ilGenerator.LoadLocal(invocationLocal).LoadLocal(parameterLocal).Callvirt(set_Parameter);
                ilGenerator.ForEach(returnValueInvocations, (_, invocation, index) => _
                        .LoadLocal(interceptorLocal).LoadArrayItem(index).LoadLocal(invocationLocal).Callvirt(on_Intercept));
                ilGenerator.LoadLocal(parameterLocal).Callvirt(get_Value).StoreLocal(local[3]);
                //}).
                //Catch(il => ExcepionIntercept(il, interceptors, local)).EndException();
            }
        }

        /// <summary>
        ///     将当前泛型方法定义的类型参数替换为类型数组的元素
        /// </summary>
        internal static ILGenerator MakeGenericMethod(ILGenerator ilGenerator, MethodInfo method, LocalBuilder interceptedMethodLocal)
        {
            if (method.IsGenericMethod)
            {
                var genericArguments = method.GetGenericArguments();
                var genericArgumentsLocal = ilGenerator.DeclareLocal(typeof(Type[]));
                var makeGenericMethod = typeof(MethodInfo).GetMethod("MakeGenericMethod");
                ilGenerator.NewArray(typeof(Type), genericArguments.Length).StoreLocal(genericArgumentsLocal).
                    ForEach(genericArguments, (_, arg, i) =>
                    ilGenerator.LoadLocal(genericArgumentsLocal).LoadInt(i).Typeof(arg).SetArrayItemRef()).
                    LoadLocal(interceptedMethodLocal).LoadLocal(genericArgumentsLocal).
                    Callvirt(makeGenericMethod).StoreLocal(interceptedMethodLocal);
            }
            return ilGenerator;
        }

        /// <summary>
        ///     定义重写的方法
        /// </summary>
        /// <param name="typeBuilder"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        internal static MethodBuilder DefineOverrideMethod(this TypeBuilder typeBuilder, MethodInfo method)
        {
            var builder= typeBuilder.DefineMethod(method.Name, method.GetAttributes(), method.CallingConvention, method.ReturnType, method.GetParameterTypes().ToArray());
            if(method.IsGenericMethod)
            {
                return builder.DefineGeneric(method);
            }
            return builder;
        }

        /// <summary>
        ///     定义显式实现的接口方法
        /// </summary>
        internal static MethodBuilder DefineExplicitInterfaceMethod(this TypeBuilder typeBuilder, MethodInfo method)
        {
            var builder = typeBuilder.DefineMethod(method.DeclaringType.FullName + "." + method.Name,
                MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                method.CallingConvention, method.ReturnType, method.GetParameterTypes().ToArray());          
            if (method.IsGenericMethod)
            {
                 builder.DefineGeneric(method);
            }
            typeBuilder.DefineMethodOverride(builder, method);
            return builder;
        }

        /// <summary>
        ///     根据指定的方法，在动态方法中添加泛型参数和泛型约束
        /// </summary>
        internal static MethodBuilder DefineGeneric(this MethodBuilder methodBuilder, MethodInfo method)
        {
            var genericArguments = method.GetGenericArguments();
            var genericArgumentsBuilders = methodBuilder.DefineGenericParameters(genericArguments.Select(a => a.Name).ToArray());
            genericArguments.ForEach((arg, index) =>
            {
                genericArgumentsBuilders[index].SetGenericParameterAttributes(arg.GenericParameterAttributes);
                arg.GetGenericParameterConstraints().ForEach(constraint =>
                {
                    if (constraint.IsClass) genericArgumentsBuilders[index].SetBaseTypeConstraint(constraint);
                    if (constraint.IsInterface) genericArgumentsBuilders[index].SetInterfaceConstraints(constraint);
                });
            });
            return methodBuilder;
        }

        /// <summary>
        ///     向类型中添加属性的方法
        /// </summary>      
        internal static TypeBuilder DefineProperty(this TypeBuilder builder, PropertyInfo property)
        {
            var attributes = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual;

            var filed = builder.DefineField("_" + property.Name, property.PropertyType, FieldAttributes.Private);

            var propertyBuilder = builder.DefineProperty(property.Name, property.Attributes, property.PropertyType, null);

            var getMethod = builder.DefineMethod("get_" + property.Name, attributes, property.PropertyType, null);
            getMethod.GetILGenerator().Method(il => il.GetFiled(filed));

            var setMethod = builder.DefineMethod("set_" + property.Name, attributes, typeof(void), new[] { property.PropertyType });
            setMethod.GetILGenerator().Method(il => il.SetFiled(filed));

            propertyBuilder.SetGetMethod(getMethod);
            propertyBuilder.SetSetMethod(setMethod);

            return builder;
        }
      
        /// <summary>
        ///     获取重写方法的属性
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static MethodAttributes GetAttributes(this MethodInfo method)
        {
            MethodAttributes attributes = MethodAttributes.Virtual;
            if (method.IsPublic)
                attributes = attributes | MethodAttributes.Public;
            if (method.IsFamily)
                attributes = attributes | MethodAttributes.Family;
            if (method.IsFamilyOrAssembly)
                attributes = attributes | MethodAttributes.FamORAssem;
            if (method.IsAssembly)
                attributes = attributes | MethodAttributes.Assembly;
            if (method.IsHideBySig)
                attributes = attributes | MethodAttributes.HideBySig;
            if (method.IsSpecialName)
                attributes = attributes | MethodAttributes.SpecialName;
            return attributes;
        }
    }
}