using System;
using System.Reflection;

namespace FS.DI.DynamicProxy
{
    public interface IParameter
    {
        Object Value { get; set; }
        string Name { get; }
        ParameterInfo ParameterInfo { get; }
    }
}
