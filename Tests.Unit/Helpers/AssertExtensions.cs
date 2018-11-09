using System;
using JetBrains.Annotations;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers
{
    public static class AssertExtensions
    {
        public static T AssertOfType<T>([CanBeNull] this object value)
        {
            if (value is T t)
                return t;

            Assert.True(false, $"Object of type {value?.GetType()?.FullName} is not of type {typeof(T).FullName}");

            throw new InvalidOperationException("Not reachable");
        }

        public static void AssertOfType([CanBeNull] this object value, [NotNull] Type type)
        {
            Assert.NotNull(type);
            Assert.True(type.IsAssignableFrom(value?.GetType()));
        }
    }
}
