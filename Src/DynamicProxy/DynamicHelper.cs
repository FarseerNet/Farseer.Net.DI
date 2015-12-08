using FS.Extends;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace FS.DI.DynamicProxy
{
    /// <summary>
    ///     
    /// </summary>
    internal static class DynamicHelper
    {
        private static string DynamicTypeName(this Type parentType) => $"_Dynamic{parentType.Name}";

        internal static TypeBuilder DefineProxyType(this ModuleBuilder moduleBuilder, Type parentType)
        {
            if (parentType == null) throw new ArgumentNullException(nameof(parentType));
            if (parentType.IsInterface || parentType.IsAbstract)
                throw new InvalidOperationException($"接口或抽象类\"{parentType.FullName}\"不能生成代理类。");
            if (parentType.IsNotPublic)
                throw new InvalidOperationException($"非公共类型\"{parentType.FullName}\"不能生成代理类。");
            if (parentType.IsSealed)
                throw new InvalidOperationException($"封闭类型\"{parentType.FullName}\"不能生成代理类。");

            return moduleBuilder.DefineType(parentType.DynamicTypeName(), parentType.Attributes, parentType,
                parentType.GetInterfaces());
        }


        internal static TypeBuilder DefineConstructors(this TypeBuilder typeBuilder, Type parentType)
        {
            parentType.GetConstructors().ForEach(constructor => typeBuilder.DefineConstructor(constructor.Attributes, constructor.CallingConvention,
                   constructor.GetParameterTypes().ToArray()).GetILGenerator().CallBaseConstructor(constructor).Return());
            return typeBuilder;
        }


        internal static TypeBuilder DefineOverrideMethods(this TypeBuilder typeBuilder, Type parentType)
        {
            var methods = parentType.GetProxyMethods();
            var getInterceptedMethod = typeof(InterceptorExtends).GetMethod("GetInterceptedMethod", Type.EmptyTypes);
            var getInterceptorsMethod = typeof(InterceptorExtends).GetMethod("GetInterceptors");
            var getMakeGenericMethod = typeof(InterceptorExtends).GetMethod("MakeGenericMethod");
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
                    var interceptedTypeLocal = ilGenerator.DeclareLocal(typeof(Type));
                    if (returnValue) returnLocal = ilGenerator.DeclareLocal(method.ReturnType);
                    ilGenerator.GetBaseType().StoreLocal(interceptedTypeLocal).Call(getInterceptedMethod);
                    if (method.IsGenericMethod) ilGenerator.Call(getMakeGenericMethod);
                    ilGenerator.StoreLocal(interceptedMethodLocal).LoadLocal(interceptedMethodLocal).LoadLocal(interceptedTypeLocal)
                    .Call(getInterceptorsMethod).StoreLocal(interceptorsLocal);
                    MakeGenericMethod(ilGenerator, method, interceptedMethodLocal);
                    ilGenerator.Try(il =>
                    {
                        ParameterIntercept(ilGenerator, interceptors, new[] { interceptorsLocal, interceptedMethodLocal, interceptedTypeLocal, returnLocal }, method);
                        MethodIntercept(ilGenerator, method, interceptors, new[] { interceptorsLocal, interceptedMethodLocal, interceptedTypeLocal, returnLocal },
                        new[] { returnValue }, _ =>
                        {
                            ilGenerator.CallBase(method);
                            if (returnValue) ilGenerator.StoreLocal(returnLocal);
                        });
                    }).
                    Catch(il => ExcepionIntercept(il, interceptors, new[] { interceptorsLocal, interceptedMethodLocal, interceptedTypeLocal }))
                    .EndException();
                    if (returnValue) ilGenerator.LoadLocal(returnLocal);
                    ilGenerator.Return();
                }
            }
            return typeBuilder;
        }

        internal static TypeBuilder DefineExplicitInterfaceMethods(this TypeBuilder typeBuilder, Type interfaceType,
            Type parentType)
        {
            var methods = interfaceType.GetProxyMethods();
            var getInterfaceMethod = typeof(Type).GetMethod("GetInterface", new Type[] { typeof(string) });
            var getInterceptorsMethod = typeof(InterceptorExtends).GetMethod("GetInterceptors");
            var getInterceptedIMethod = typeof(InterceptorExtends).GetMethod("GetIntercepted_IMethod", new[] { typeof(string) });
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
                    var interceptedTypeLocal = ilGenerator.DeclareLocal(typeof(Type));
                    if (returnValue) returnLocal = ilGenerator.DeclareLocal(method.ReturnType);
                    ilGenerator.LoadString(interfaceType.Name).Call(getInterceptedIMethod);
                    ilGenerator.StoreLocal(interceptedMethodLocal).GetThisType().LoadString(interfaceType.Name)
                    .Callvirt(getInterfaceMethod).StoreLocal(interceptedTypeLocal);
                    ilGenerator.LoadLocal(interceptedMethodLocal).LoadLocal(interceptedTypeLocal).Call(getInterceptorsMethod).StoreLocal(interceptorsLocal);
                    MakeGenericMethod(ilGenerator, method, interceptedMethodLocal);
                    ilGenerator.Try(il =>
                    {
                        ParameterIntercept(ilGenerator, interceptors, new[] { interceptorsLocal, interceptedMethodLocal, interceptedTypeLocal, returnLocal }, method);
                        MethodIntercept(ilGenerator, method, interceptors, new[] { interceptorsLocal, interceptedMethodLocal, interceptedTypeLocal, returnLocal },
                        new bool[] { returnValue }, _ =>
                        {
                            var baseMethod = parentType.GetMethods().First(m => m.EqualMethod(method));
                            if (baseMethod == null) throw new NotImplementedException(parentType.FullName + " 未实现方法 " + method.ToString());
                            ilGenerator.CallBase(baseMethod);
                            if (returnValue) ilGenerator.StoreLocal(returnLocal);
                        });
                    }).
                    Catch(il => ExcepionIntercept(il, interceptors, new[] { interceptorsLocal, interceptedMethodLocal, interceptedTypeLocal }))
                    .EndException();
                    if (returnValue) ilGenerator.LoadLocal(returnLocal);
                    ilGenerator.Return();
                }
            }
            return typeBuilder;
        }

        private static void ParameterIntercept(ILGenerator ilGenerator, IInterceptor[] interceptors,
            LocalBuilder[] local, MethodInfo method)
        {
            var parameterInterceptors = interceptors.GetParameterInterceptors();
            if (!parameterInterceptors.Any()) return;
            var invocationType = InternalDynamicTypeProvider.CreateType<IParameterInvocation>();
            var parameterType = InternalDynamicTypeProvider.CreateType<IParameter>();
            var setInterceptedMethod = invocationType.GetMethod("set_InterceptedMethod");
            var setInterceptedType = invocationType.GetMethod("set_InterceptedType");
            var setInterceptedInstance = invocationType.GetMethod("set_InterceptedInstance");
            var setParameters = invocationType.GetMethod("set_Parameters");
            var setValue = parameterType.GetMethod("set_Value");
            var getValue = parameterType.GetMethod("get_Value");
            var setName = parameterType.GetMethod("set_Name");
            var setParameterInfo = parameterType.GetMethod("set_ParameterInfo");
            var getName = typeof(ParameterInfo).GetMethod("get_Name");
            var getParameterInterceptors = typeof(InterceptorExtends).GetMethod("GetParameterInterceptors");
            var getCurrentParameter = typeof(InterceptorExtends).GetMethod("GetCurrentParameter");
            var onIntercept = typeof(IParameterInterceptor).GetMethod("OnParameterExecuting");
            var interceptorLocal = ilGenerator.DeclareLocal(typeof(IParameterInterceptor[]));
            var parametersLocal = ilGenerator.DeclareLocal(typeof(IParameter[]));
            ilGenerator.LoadLocal(local[0]).Call(getParameterInterceptors).StoreLocal(interceptorLocal);
            var invocationLocal = ilGenerator.DeclareLocal(invocationType);
            ilGenerator.New(invocationType.GetConstructor(Type.EmptyTypes)).StoreLocal(invocationLocal);
            ilGenerator.LoadLocal(invocationLocal).LoadLocal(local[1]).Callvirt(setInterceptedMethod);
            ilGenerator.LoadLocal(invocationLocal).LoadLocal(local[2]).Callvirt(setInterceptedType);
            ilGenerator.LoadLocal(invocationLocal).This().Callvirt(setInterceptedInstance);
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
                        ilGenerator.LoadLocal(argLocal).LoadArgument(k + 1).Box(arg.ParameterType).Callvirt(setValue);
                        ilGenerator.LoadLocal(argLocal).LoadLocal(local[1]).LoadInt(k).Call(getCurrentParameter).Callvirt(setParameterInfo);
                        ilGenerator.LoadLocal(argLocal).LoadLocal(local[1]).LoadInt(k).Call(getCurrentParameter).Callvirt(getName).Callvirt(setName);
                        ilGenerator.LoadLocal(parametersLocal).LoadInt(j).LoadLocal(argLocal).SetArrayItemRef();
                    }
                }));
                ilGenerator.LoadLocal(invocationLocal).LoadLocal(parametersLocal).Callvirt(setParameters);
                ilGenerator.LoadLocal(interceptorLocal).LoadArrayItem(i).LoadLocal(invocationLocal).Callvirt(onIntercept);
                ilGenerator.ForEach(parameters, (__, parameter, j) =>
                ilGenerator.ForEach(method.GetParameters(), (___, arg, k) =>
                {
                    if (arg == parameter)
                    {
                        ilGenerator.LoadLocal(parametersLocal).LoadArrayItem(j).Callvirt(getValue).UnBox(arg.ParameterType).StroeArgument(k + 1);
                    }
                }));
            });
        }

        private static void ExcepionIntercept(ILGenerator ilGenerator, IInterceptor[] interceptors, LocalBuilder[] local)
        {
            var excepionInterceptors = interceptors.GetExcepionInterceptors();
            if (!excepionInterceptors.Any())
            {
                ilGenerator.Emit(OpCodes.Throw);
                return;
            }
            var invocationType = InternalDynamicTypeProvider.CreateType<IExceptionInvocation>();
            var setException = invocationType.GetMethod("set_Exception");
            var setInterceptedType = invocationType.GetMethod("set_InterceptedType");
            var setInterceptedInstance = invocationType.GetMethod("set_InterceptedInstance");
            var getExceptionHandled = invocationType.GetMethod("get_ExceptionHandled");
            var getExcepionInterceptMethod = typeof(IExceptionInterceptor).GetMethod("OnExcepion");
            var getExcepionInterceptorsMethod = typeof(InterceptorExtends).GetMethod("GetExcepionInterceptors");
            var exceptionLocal = ilGenerator.DeclareLocal(typeof(Exception));
            var interceptorLocal = ilGenerator.DeclareLocal(typeof(IExceptionInterceptor[]));
            var invocationLocal = ilGenerator.DeclareLocal(invocationType);
            var breakExceptionLable = ilGenerator.DefineLabel();
            ilGenerator.StoreLocal(exceptionLocal).LoadLocal(local[0]).Call(getExcepionInterceptorsMethod).StoreLocal(interceptorLocal);
            ilGenerator.New(invocationType.GetConstructor(Type.EmptyTypes)).StoreLocal(invocationLocal);
            ilGenerator.LoadLocal(invocationLocal).LoadLocal(exceptionLocal).Callvirt(setException);
            ilGenerator.LoadLocal(invocationLocal).LoadLocal(local[2]).Callvirt(setInterceptedType);
            ilGenerator.LoadLocal(invocationLocal).This().Callvirt(setInterceptedInstance);
            ilGenerator.ForEach(excepionInterceptors, (il, interceptor, index) =>
            il.LoadLocal(invocationLocal).Callvirt(getExceptionHandled).False(breakExceptionLable).LoadLocal(interceptorLocal).LoadArrayItem(index).
            LoadLocal(invocationLocal).Callvirt(getExcepionInterceptMethod)).MarkLabelFor(breakExceptionLable);
        }

        private static void MethodIntercept(ILGenerator ilGenerator, MethodInfo methodInfo, IInterceptor[] interceptors, LocalBuilder[] local,
            bool[] boolean, Action<ILGenerator> method)
        {
            var methodInterceptors = interceptors.GetMethodInterceptor();
            if (!methodInterceptors.Any())
            {
                method(ilGenerator);
                return;
            }
            var invocationType = InternalDynamicTypeProvider.CreateType<IMethodInvocation>();
            var returnParameterType = InternalDynamicTypeProvider.CreateType<IReturnParameter>();

            var parameterType = InternalDynamicTypeProvider.CreateType<IParameter>();
            var setParameters = invocationType.GetMethod("set_Parameters");
            var setParameterValue = parameterType.GetMethod("set_Value");
            var getParameterValue = parameterType.GetMethod("get_Value");
            var setParameterName = parameterType.GetMethod("set_Name");
            var setParameterInfo = parameterType.GetMethod("set_ParameterInfo");
            var getParameterName = typeof(ParameterInfo).GetMethod("get_Name");
            var getCurrentParameter = typeof(InterceptorExtends).GetMethod("GetCurrentParameter");

            var setReturnParameter = invocationType.GetMethod("set_ReturnParameter");
            var getReturnValue = returnParameterType.GetMethod("get_Value");
            var setReturnType = returnParameterType.GetMethod("set_ReturnType");
            var setReturnValue = returnParameterType.GetMethod("set_Value");
            var returnParameterLocal = ilGenerator.DeclareLocal(returnParameterType);
            var getReturnType = typeof(MethodInfo).GetMethod("get_ReturnType");
            var setInterceptedMethod = invocationType.GetMethod("set_InterceptedMethod");
            var setInterceptedType = invocationType.GetMethod("set_InterceptedType");
            var setInterceptedInstance = invocationType.GetMethod("set_InterceptedInstance");
            var getExecutedHandled = invocationType.GetMethod("get_ExecutedHandled");
            var getGetMethodInterceptor = typeof(InterceptorExtends).GetMethod("GetMethodInterceptor");
            var getGetExecutedMethodInterceptor = typeof(InterceptorExtends).GetMethod("GetExecutedMethodInterceptor");
            var onMethodExecuting = typeof(IMethodInterceptor).GetMethod("OnMethodExecuting");
            var onMethodExecuted = typeof(IMethodInterceptor).GetMethod("OnMethodExecuted");
            var interceptorLocal = ilGenerator.DeclareLocal(typeof(IMethodInterceptor[]));
            var executedInterceptorLocal = ilGenerator.DeclareLocal(typeof(IMethodInterceptor[]));


            var parametersLocal = ilGenerator.DeclareLocal(typeof(IParameter[]));
         


            var invocationLocal = ilGenerator.DeclareLocal(invocationType);
            var endLable = ilGenerator.DefineLabel();
            ilGenerator.LoadLocal(local[0]).Call(getGetMethodInterceptor).StoreLocal(interceptorLocal);
            ilGenerator.LoadLocal(local[0]).Call(getGetExecutedMethodInterceptor).StoreLocal(executedInterceptorLocal);
            ilGenerator.New(invocationType.GetConstructor(Type.EmptyTypes)).StoreLocal(invocationLocal);
            ilGenerator.LoadLocal(invocationLocal).LoadLocal(local[1]).Callvirt(setInterceptedMethod);
            ilGenerator.LoadLocal(invocationLocal).LoadLocal(local[2]).Callvirt(setInterceptedType);
            ilGenerator.LoadLocal(invocationLocal).This().Callvirt(setInterceptedInstance);
            ilGenerator.New(returnParameterType.GetConstructor(Type.EmptyTypes)).StoreLocal(returnParameterLocal);
            if (boolean[0]) ilGenerator.LoadLocal(returnParameterLocal).LoadLocal(local[1]).Callvirt(getReturnType).Callvirt(setReturnType);
            else ilGenerator.LoadLocal(returnParameterLocal).Typeof(typeof(void)).Callvirt(setReturnType);
            ilGenerator.LoadLocal(invocationLocal).LoadLocal(returnParameterLocal).Callvirt(setReturnParameter);

            ilGenerator.NewArray(typeof(IParameter), methodInfo.GetParameters().Length).StoreLocal(parametersLocal);
            ilGenerator.ForEach(methodInfo.GetParameterTypes(), (il, arg, i) =>
            {
                var argLocal = ilGenerator.DeclareLocal(parameterType);
                ilGenerator.New(parameterType.GetConstructor(Type.EmptyTypes)).StoreLocal(argLocal);
                ilGenerator.LoadLocal(argLocal).LoadArgument(i + 1).Box(arg).Callvirt(setParameterValue);
                ilGenerator.LoadLocal(argLocal).LoadLocal(local[1]).LoadInt(i).Call(getCurrentParameter).Callvirt(setParameterInfo);
                ilGenerator.LoadLocal(argLocal).LoadLocal(local[1]).LoadInt(i).Call(getCurrentParameter).Callvirt(getParameterName).Callvirt(setParameterName);
                ilGenerator.LoadLocal(parametersLocal).LoadInt(i).LoadLocal(argLocal).SetArrayItemRef();
            });
            ilGenerator.LoadLocal(invocationLocal).LoadLocal(parametersLocal).Callvirt(setParameters);

            ilGenerator.ForEach(methodInterceptors, (_, interceptor, index) =>
            _.LoadLocal(interceptorLocal).LoadArrayItem(index).LoadLocal(invocationLocal)
            .Callvirt(onMethodExecuting).LoadLocal(invocationLocal).Callvirt(getExecutedHandled).False(endLable));

            ilGenerator.ForEach(methodInfo.GetParameterTypes(), (il, arg, i) =>
            {
                ilGenerator.LoadLocal(parametersLocal).LoadArrayItem(i).Callvirt(getParameterValue).UnBox(arg).StroeArgument(i + 1);
            });
            method(ilGenerator);


            if (boolean[0]) ilGenerator.LoadLocal(returnParameterLocal).LoadLocal(local[3]).Callvirt(setReturnValue);
            ilGenerator.ForEach(methodInterceptors.OrderBy(i => i.ExecutedOrder), (_, interceptor, index) =>
            _.LoadLocal(executedInterceptorLocal).LoadArrayItem(index).LoadLocal(invocationLocal).
            Callvirt(onMethodExecuted).LoadLocal(invocationLocal).Callvirt(getExecutedHandled).False(endLable));
            ilGenerator.MarkLabelFor(endLable);
            if (boolean[0]) ilGenerator.LoadLocal(returnParameterLocal).Callvirt(getReturnValue).StoreLocal(local[3]);
        }

        private static ILGenerator MakeGenericMethod(ILGenerator ilGenerator, MethodInfo method,
            LocalBuilder interceptedMethodLocal)
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

        private static MethodBuilder DefineOverrideMethod(this TypeBuilder typeBuilder, MethodInfo method)
        {
            var builder = typeBuilder.DefineMethod(method.Name, method.GetAttributes(), method.CallingConvention, method.ReturnType, method.GetParameterTypes().ToArray());
            if (method.IsGenericMethod)
            {
                return builder.DefineGeneric(method);
            }
            return builder;
        }

        private static MethodBuilder DefineExplicitInterfaceMethod(this TypeBuilder typeBuilder, MethodInfo method)
        {
            var builder = typeBuilder.DefineMethod($"{method.DeclaringType.FullName}.{method.Name}",
                MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual,
                method.CallingConvention, method.ReturnType, method.GetParameterTypes().ToArray());
            if (method.IsGenericMethod)
            {
                builder.DefineGeneric(method);
            }
            typeBuilder.DefineMethodOverride(builder, method);
            return builder;
        }


        private static MethodBuilder DefineGeneric(this MethodBuilder methodBuilder, MethodInfo method)
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


        internal static TypeBuilder DefineProperty(this TypeBuilder builder, PropertyInfo property)
        {
            var attributes = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.SpecialName |
                             MethodAttributes.NewSlot | MethodAttributes.Virtual;

            var filed = builder.DefineField("_" + property.Name, property.PropertyType, FieldAttributes.Private);

            var propertyBuilder = builder.DefineProperty(property.Name, property.Attributes, property.PropertyType, null);

            var getMethod = builder.DefineMethod("get_" + property.Name, attributes, property.PropertyType, null);
            getMethod.GetILGenerator().Method(il => il.GetFiled(filed));

            var setMethod = builder.DefineMethod("set_" + property.Name, attributes, typeof (void),
                new[] {property.PropertyType});
            setMethod.GetILGenerator().Method(il => il.SetFiled(filed));

            propertyBuilder.SetGetMethod(getMethod);
            propertyBuilder.SetSetMethod(setMethod);

            return builder;
        }


        private static MethodAttributes GetAttributes(this MethodInfo method)
        {
            var attributes = MethodAttributes.Virtual;
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