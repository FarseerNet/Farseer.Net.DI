using FS.DI.Core;
using FS.Extends;
using FS.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace FS.DI.Register
{
    /// <summary>
    /// DependencyRegistrationFactory
    /// </summary>
    internal static class DependencyRegistrationFactory
    {
        internal static IDependencyRegistration ForType(Type serviceType, Type implementationType)
        {
            return new DependencyRegistration(
                DependencyEntry.ForType(serviceType, DependencyLifetime.Transient, implementationType));
        }

        internal static IDependencyRegistration ForInstance(Type serviceType, Object implementationInstance)
        {
            return new DependencyRegistration(
               DependencyEntry.ForInstance(serviceType, implementationInstance));
        }

        internal static IDependencyRegistration ForDelegate<TService>(Type serviceType, Func<IDependencyResolver, TService> implementationDelegate)
             where TService : class
        {
            return new DependencyRegistration(
               DependencyEntry.ForDelegate(serviceType, DependencyLifetime.Transient, implementationDelegate));
        }

        internal static IEnumerableRegistration ForAssembly(Assembly assembly, Type baseType)
        {
            return ForAssembly(assembly,
                type => baseType.IsGenericTypeDefinition ? type.GetGenericTypeDefinitions().Any(genericType => genericType == baseType) : baseType.IsAssignableFrom(type),
                type => (baseType.IsInterface ? !baseType.GetInterfacesTypes().Contains(type) : !baseType.GetInterfacesTypes().Concat(baseType.GetBaseTypes()).Contains(type)));
        }
      
        internal static IEnumerableRegistration ForAssembly(Assembly assembly, String name)
        {
            Func<Type, bool> filter = type => type.Name.Contains(name);
            return ForAssembly(assembly, filter, filter);
        }

        internal static IEnumerableRegistration ForAssembly(Assembly assembly, Func<Type, bool> typeFilter, Func<Type, bool> serviceTypeFilter = null)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            
            var registerTypes = new TypeFinder(assembly).Find(typeFilter);

            return new EnumerableRegistration(
                GetRegistrationCollection(registerTypes, serviceTypeFilter));
        }

        internal static IEnumerableRegistration ForAssembly(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));

            var registerTypes = new TypeFinder(assembly).FindAll();

            return new EnumerableRegistration(
                GetRegistrationCollection(registerTypes));
        }

        private static IEnumerable<IDependencyRegistration> GetRegistrationCollection(Type[] types, Func<Type, bool> serviceTypeFilter = null)
        {
            if (types == null) throw new ArgumentNullException(nameof(types));

            foreach (Type type in types)
            {
                if (type.IsAbstract || type.IsInterface)
                {
                    continue;
                }

                var serviceTypes = type.GetInterfacesTypes().Concat(type.GetBaseTypes());

                yield return ForType(type, type);

                foreach (var serviceType in serviceTypes)
                {
                    if (serviceTypeFilter != null && !serviceTypeFilter(serviceType))
                        continue;

                    yield return ForType(serviceType, type);
                }
            }
        }
    }
}
