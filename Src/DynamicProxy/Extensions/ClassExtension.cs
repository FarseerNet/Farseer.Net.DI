using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Linq = System.Linq.Expressions;

namespace FS.DI.DynamicProxy.Expressions
{
    public class ClassExtension : Extension
    {
        public string Name { get; }
        public Extension Parent { get; }
        public Extension[] Interfaces { get; }
        public Type Type { get; }

        internal ClassExtension(Type type)
        {
            this.Type = type;
        }

        internal ClassExtension(string name, Extension parent, Extension[] interfaces)
        {
            this.Name = name;
            this.Parent = parent;
            this.Interfaces = interfaces;
        }
    }

    public partial class Extension
    {
        public static ClassExtension Class(Type type)
        {
            return new ClassExtension(type);
        }

        public static ClassExtension Class(string name, Extension parent, Extension[] interfaces)
        {
            return new ClassExtension(name, parent, interfaces);
        }
    }
}
