using FsCheck;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.UnitTests.Helpers
{
    public static class GeneratorExtensions
    {
        [NotNull]
        public static WeightAndValue<Gen<T>> WithWeight<T>(
            [NotNull] this Gen<T> generator,
            int weight)
        {
            return new WeightAndValue<Gen<T>>(weight, generator);
        }
    }
}
