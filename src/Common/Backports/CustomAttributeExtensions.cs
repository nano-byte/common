// Copyright Bastian Eicher
// Licensed under the MIT License

#if NET20 || NET40
using System.Collections.Generic;
using System.Linq;

namespace System.Reflection
{
    public static class CustomAttributeExtensions
    {
        public static T? GetCustomAttribute<T>(this Assembly element) where T : Attribute
            => element.GetCustomAttributes<T>().FirstOrDefault();

        public static IEnumerable<T> GetCustomAttributes<T>(this Assembly element) where T : Attribute
            => element.GetCustomAttributes(typeof(T), inherit: true).OfType<T>();

        public static T? GetCustomAttribute<T>(this MemberInfo element) where T : Attribute
            => element.GetCustomAttributes<T>().FirstOrDefault();

        public static IEnumerable<T> GetCustomAttributes<T>(this MemberInfo element) where T : Attribute
            => element.GetCustomAttributes(typeof(T), inherit: true).OfType<T>();
    }
}
#else
[assembly: System.Runtime.CompilerServices.TypeForwardedTo(typeof(System.Reflection.CustomAttributeExtensions))]
#endif
