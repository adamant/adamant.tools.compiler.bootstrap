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
    }
}
