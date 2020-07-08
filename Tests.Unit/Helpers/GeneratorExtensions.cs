using System;
using System.Runtime.CompilerServices;
using FsCheck;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Helpers
{
    public static class GeneratorExtensions
    {
        public static WeightAndValue<Gen<T>> WithWeight<T>(
            this Gen<T> generator,
            int weight)
        {
            return new WeightAndValue<Gen<T>>(weight, generator);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]

        public static T NotNull<T>(this NonNull<T> value, string paramName)
            where T : class
        {
            return (value ?? throw new ArgumentNullException(paramName)).Get;
        }
    }
}
