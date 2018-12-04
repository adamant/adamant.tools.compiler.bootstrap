using System;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers
{
    public static class AssertExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static T NotNull<T>([CanBeNull] this FsCheck.NonNull<T> value)
            where T : class
        {
            return (value ?? throw new ArgumentNullException()).Get;
        }

        [NotNull]
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

        public static void AssertDiagnostic(
            [CanBeNull] this Diagnostic diagnostic,
            DiagnosticPhase phase,
            int errorCode,
            int start,
            int length)
        {
            Assert.NotNull(diagnostic);
            Assert.Equal(phase, diagnostic.Phase);
            Assert.Equal(errorCode, diagnostic.ErrorCode);
            Assert.True(start == diagnostic.Span.Start, $"Expected diagnostic start {start}, was {diagnostic.Span.Start}");
            Assert.True(length == diagnostic.Span.Length, $"Expected diagnostic length {length}, was {diagnostic.Span.Length}");
        }
    }
}
