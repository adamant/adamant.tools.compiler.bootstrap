using System;
using System.Linq;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Framework
{
    public static class TypeExtensions
    {
        public static string GetFriendlyName([NotNull] this Type type)
        {
            if (type.IsGenericParameter || !type.IsGenericType)
                return type.Name;

            var name = type.Name.NotNull();
            var index = name.IndexOf("`", StringComparison.Ordinal);
            name = name.Substring(0, index);
            var genericArguments = string.Join(',', type.GetGenericArguments().NotNull().Select(GetFriendlyName));
            return $"{name}<{genericArguments}>";
        }
    }
}
