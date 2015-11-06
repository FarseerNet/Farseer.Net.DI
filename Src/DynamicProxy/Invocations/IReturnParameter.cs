using System;

namespace FS.DI.DynamicProxy
{
    public interface IReturnParameter
    {
        Object Value { get; set; }

        Type ReturnType { get; }
    }
}
