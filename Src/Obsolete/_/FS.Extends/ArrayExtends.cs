using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FS.Extends
{
    public static class ArrayExtends
    {
        public static T[] Empty<T>(this Array array)
        {
            return Empty<T>();
        }

        public static T[] Empty<T>()
        {
            return new T[0];
        }
    }
}
