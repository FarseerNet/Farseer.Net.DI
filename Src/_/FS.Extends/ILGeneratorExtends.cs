using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using FS.Extends;
using System.Diagnostics;

namespace FS.DI.DynamicProxy
{
    /// <summary>
    ///     一个代码很乱的类，请忽略。
    /// </summary>
    internal static class ILGeneratorExtends
    {
        /// <summary>
        ///     调用基类构造方法
        /// </summary>
        internal static ILGenerator CallBaseConstructor(this ILGenerator ilGenerator, ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameterTypes().ToArray();
            ilGenerator.LoadArgument(0);
            if (parameters.Any())
            {
                for (var index = 1; index <= parameters.Length; index++)
                {
                    ilGenerator.LoadArgument(index);
                }
            }
            ilGenerator.Emit(OpCodes.Call, constructor);
            return ilGenerator;
        }

        /// <summary>
        ///     方法返回
        /// </summary>
        internal static ILGenerator Return(this ILGenerator ilGenerator)
        {
            ilGenerator.Emit(OpCodes.Ret);
            return ilGenerator;
        }

        /// <summary>
        ///     返回this指针
        /// </summary>
        internal static ILGenerator This(this ILGenerator ilGenerator)
        {
            return ilGenerator.LoadArgument(0);
        }

        /// <summary>
        ///     加载指定索引处的参数
        /// </summary>
        internal static ILGenerator LoadArgument(this ILGenerator ilGenerator, int index)
        {
            switch (index)
            {
                case 0:
                    ilGenerator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    ilGenerator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    ilGenerator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    ilGenerator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (index > -129 && index < 128)
                        ilGenerator.Emit(OpCodes.Ldarg_S, (sbyte)index);
                    else
                        ilGenerator.Emit(OpCodes.Ldarg, index);
                    break;
            }
            return ilGenerator;
        }

        private static MethodInfo _getTypeMethod = typeof(object).GetMethod("GetType");
        private static MethodInfo _getBaseTypeMethod = typeof(Type).GetMethod("get_BaseType");

        /// <summary>
        ///     获取Type
        /// </summary>
        internal static ILGenerator GetThisType(this ILGenerator ilGenerator)
        {
            ilGenerator.This();
            ilGenerator.Call(_getTypeMethod);
            return ilGenerator;
        }

        /// <summary>
        ///     获取基类
        /// </summary>
        internal static ILGenerator GetBaseType(this ILGenerator ilGenerator)
        {
            ilGenerator.GetThisType();
            ilGenerator.Callvirt(_getBaseTypeMethod);
            return ilGenerator;
        }

        private static MethodInfo _stackFrameMethod = typeof(StackFrame).GetMethod("GetMethod");
        private static ConstructorInfo _stackFrameConstructor = typeof(StackFrame).GetConstructors().Where(ctor => ctor.GetParameters().Length == 0).Single();
        /// <summary>
        ///     获取当前调用的方法
        /// </summary>
        internal static ILGenerator GetStackFrameMethod(this ILGenerator ilGenerator)
        {
            ilGenerator.Emit(OpCodes.Newobj, _stackFrameConstructor);
            ilGenerator.EmitCall(OpCodes.Callvirt, _stackFrameMethod, null);
            return ilGenerator;
        }

        /// <summary>
        ///     声明指定类型的局部变量并存储在指定索引处的局部变量列表中
        /// </summary>
        internal static ILGenerator DeclareLocal(this ILGenerator ilGenerator, Type type, int index)
        {
            ilGenerator.DeclareLocal(type);
            ilGenerator.DeclareLocal(index);
            return ilGenerator;
        }

        /// <summary>
        ///     将局部变量存储在指定索引处的局部变量列表中
        /// </summary>
        internal static ILGenerator DeclareLocal(this ILGenerator ilGenerator, int index)
        {
            switch (index)
            {
                case 0:
                    ilGenerator.Emit(OpCodes.Stloc_0);
                    break;
                case 1:
                    ilGenerator.Emit(OpCodes.Stloc_1);
                    break;
                case 2:
                    ilGenerator.Emit(OpCodes.Stloc_2);
                    break;
                case 3:
                    ilGenerator.Emit(OpCodes.Stloc_3);
                    break;
                default:
                    if (index <= 0xff)
                        ilGenerator.Emit(OpCodes.Stloc_S, (sbyte)index);
                    else
                        ilGenerator.Emit(OpCodes.Stloc, index);
                    break;
            }
            return ilGenerator;
        }

        /// <summary>
        ///     加载指定索引处的局部变量
        /// </summary>
        internal static ILGenerator LoadLocal(this ILGenerator ilGenerator, int index)
        {
            switch (index)
            {
                case 0:
                    ilGenerator.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    ilGenerator.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    ilGenerator.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    ilGenerator.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    if (index <= 0xff)
                        ilGenerator.Emit(OpCodes.Ldloc_S, (sbyte)index);
                    else
                        ilGenerator.Emit(OpCodes.Ldloc, index);
                    break;
            }
            return ilGenerator;
        }

        /// <summary>
        ///     加载指定索引处的局部变量
        /// </summary>
        internal static ILGenerator LoadLocal(this ILGenerator ilGenerator, LocalBuilder local)
        {
            ilGenerator.Emit(OpCodes.Ldloc, local);
            return ilGenerator;
        }

        /// <summary>
        /// 从计算堆栈的顶部弹出当前值并将其存储到指定索引处的局部变量列表中。
        /// </summary>
        internal static ILGenerator StoreLocal(this ILGenerator ilGenerator, LocalBuilder local)
        {
            ilGenerator.Emit(OpCodes.Stloc, local);
            return ilGenerator;
        }

        /// <summary>
        ///     加载指定索引处的数组变量
        /// </summary>
        internal static ILGenerator LoadArrayItem(this ILGenerator ilGenerator, int index)
        {
            ilGenerator.LoadInt(index).Emit(OpCodes.Ldelem_Ref);
            return ilGenerator;
        }

        /// <summary>
        ///     加载int值
        /// </summary>
        public static ILGenerator LoadInt(this ILGenerator ilGenerator, int value)
        {
            switch (value)
            {
                case -1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_M1);
                    break;
                case 0:
                    ilGenerator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    ilGenerator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    ilGenerator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    ilGenerator.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    ilGenerator.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    ilGenerator.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    ilGenerator.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    ilGenerator.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    ilGenerator.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    if (value > -129 && value < 128)
                        ilGenerator.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                    else
                        ilGenerator.Emit(OpCodes.Ldc_I4, value);
                    break;
            }

            return ilGenerator;
        }

        /// <summary>
        ///     加载字符串
        /// </summary>
        public static ILGenerator LoadString(this ILGenerator ilGenerator, string value)
        {
            if (value == null)
            {
                return ilGenerator.LoadNull();
            }

            ilGenerator.Emit(OpCodes.Ldstr, value);
            return ilGenerator;
        }

        /// <summary>
        ///     加载null值
        /// </summary>
        public static ILGenerator LoadNull(this ILGenerator ilGenerator)
        {
            ilGenerator.Emit(OpCodes.Ldnull);
            return ilGenerator;
        }

        /// <summary>
        ///     try代码块
        /// </summary>
        public static ILGenerator Try(this ILGenerator ilGenerator, Action<ILGenerator> @try)
        {
            ilGenerator.BeginExceptionBlock();
            @try(ilGenerator);
            return ilGenerator;
        }

        /// <summary>
        ///     cache代码块
        /// </summary>
        public static ILGenerator Catch(this ILGenerator ilGenerator, Type exceptionType, Action<ILGenerator> @catch)
        {
            ilGenerator.BeginCatchBlock(exceptionType);
            @catch(ilGenerator);
            return ilGenerator;
        }

        /// <summary>
        ///     cache代码块
        /// </summary>
        public static ILGenerator Catch<TException>(this ILGenerator ilGenerator, Action<ILGenerator> @catch)
            where TException : Exception
        {
            return ilGenerator.Catch(typeof(TException), @catch);
        }

        /// <summary>
        ///     cache代码块 默认处理 Exception
        /// </summary>
        public static ILGenerator Catch(this ILGenerator ilGenerator, Action<ILGenerator> @catch)
        {
            return ilGenerator.Catch(typeof(Exception), @catch);
        }

        /// <summary>
        ///     结束异常处理
        /// </summary>
        public static ILGenerator EndException(this ILGenerator ilGenerator)
        {
            ilGenerator.EndExceptionBlock();
            return ilGenerator;
        }

        /// <summary>
        ///     调用方法
        /// </summary>
        internal static ILGenerator Callvirt(this ILGenerator ilGenerator, MethodInfo method)
        {
            ilGenerator.EmitCall(OpCodes.Callvirt, method, method.GetParameterTypes().ToArray());
            return ilGenerator;
        }

        /// <summary>
        ///     调用方法
        /// </summary>
        internal static ILGenerator Call(this ILGenerator ilGenerator, MethodInfo method)
        {
            ilGenerator.EmitCall(OpCodes.Call, method, method.GetParameterTypes().ToArray());
            return ilGenerator;
        }

        /// <summary>
        ///     调用基类型的指定方法
        /// </summary>
        internal static ILGenerator CallBase(this ILGenerator ilGenerator, MethodInfo method)
        {
            var parameters = method.GetParameterTypes().ToArray();
            for (int i = 0; i <= parameters.Length; i++)
                ilGenerator.LoadArgument(i);
            ilGenerator.Call(method);
            return ilGenerator;
        }

        /// <summary>
        ///     for循环调用
        /// </summary>
        internal static ILGenerator ForEach<TSource>(this ILGenerator ilGenerator, IEnumerable<TSource> source, Action<ILGenerator, TSource, int> action)
        {
            source.ForEach((item, index) => action(ilGenerator, item, index));
            return ilGenerator;
        }

        /// <summary>
        ///     方法代码段
        /// </summary>
        internal static ILGenerator Method(this ILGenerator ilGenerator, Action<ILGenerator> method)
        {
            method(ilGenerator);
            ilGenerator.Return();
            return ilGenerator;
        }

        /// <summary>
        ///     字段赋值
        /// </summary>
        internal static ILGenerator SetFiled(this ILGenerator ilGenerator, FieldInfo filed)
        {
            ilGenerator.LoadArgument(0);
            ilGenerator.LoadArgument(1);
            ilGenerator.Emit(OpCodes.Stfld, filed);
            return ilGenerator;
        }

        /// <summary>
        ///     字段取值
        /// </summary>
        internal static ILGenerator GetFiled(this ILGenerator ilGenerator, FieldInfo filed)
        {
            ilGenerator.LoadArgument(0);
            ilGenerator.Emit(OpCodes.Ldfld, filed);
            return ilGenerator;
        }

        /// <summary>
        ///     初始化新对象
        /// </summary>  
        internal static ILGenerator New(this ILGenerator ilGenerator, ConstructorInfo constructor)
        {
            ilGenerator.Emit(OpCodes.Newobj, constructor);
            return ilGenerator;
        }

        /// <summary>
        ///     用给定标签标记 Microsoft 中间语言 (MSIL) 流的当前位置。
        /// </summary>        
        internal static ILGenerator MarkLabelFor(this ILGenerator ilGenerator, Label lable)
        {
            ilGenerator.MarkLabel(lable);
            return ilGenerator;
        }

        /// <summary>
        ///     if判断如果为true
        /// </summary>
        internal static ILGenerator True(this ILGenerator ilGenerator, Label lable)
        {
            ilGenerator.LoadInt(0);
            ilGenerator.Emit(OpCodes.Ceq);
            ilGenerator.Emit(OpCodes.Brtrue, lable);
            return ilGenerator;
        }

        /// <summary>
        ///     if判断如果为false
        /// </summary>
        internal static ILGenerator False(this ILGenerator ilGenerator, Label lable)
        {
            ilGenerator.LoadInt(0);
            ilGenerator.Emit(OpCodes.Ceq);
            ilGenerator.Emit(OpCodes.Brfalse, lable);
            return ilGenerator;
        }

        private static MethodInfo _getTypeFromHandle = typeof(Type).GetMethod("GetTypeFromHandle");
        internal static ILGenerator Typeof(this ILGenerator ilGenerator, Type type)
        {
            ilGenerator.Emit(OpCodes.Ldtoken, type);
            ilGenerator.Call(_getTypeFromHandle);
            return ilGenerator;
        }

        /// <summary>
        ///     使用指定的类型初始化和长度数组
        /// </summary>
        /// <param name="ilGenerator"></param>
        /// <param name="type"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal static ILGenerator NewArray(this ILGenerator ilGenerator, Type type,int length)
        {
            ilGenerator.LoadInt(length);
            ilGenerator.Emit(OpCodes.Newarr, type);
            return ilGenerator;
        }

        /// <summary>
        ///     设置数组特定索引处的引用类型值
        /// </summary>
        internal static ILGenerator SetArrayItemRef(this ILGenerator ilGenerator)
        {
            ilGenerator.Emit(OpCodes.Stelem_Ref);
            return ilGenerator;
        }

        internal static ILGenerator Box(this ILGenerator ilGenerator, Type type)
        {
            if (type.IsValueType)
                ilGenerator.Emit(OpCodes.Box, type);
            return ilGenerator;
        }

        public static ILGenerator Cast(this ILGenerator ilGenerator, Type type)
        {
            ilGenerator.Emit(OpCodes.Castclass, type);
            return ilGenerator;
        }
    }
}
