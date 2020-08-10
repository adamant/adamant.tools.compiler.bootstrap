using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    [Trait("Category", "Symbols")]
    public class FunctionSymbolTests : SymbolTestFixture
    {
        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(20)]
        public void Arity_is_number_of_parameters(int expectedParameters)
        {
            var funcA = Func("A", @params: Params(expectedParameters));

            Assert.Equal(expectedParameters, funcA.Arity);
        }

        [Fact]
        public void Functions_with_same_parameters_and_return_type_are_equal()
        {
            var parameters = Params(DataType("T1"), DataType("T2"));
            var funcA = Func("A", @params: parameters, @return: DataType("T3"));
            var funcACopy = Func(funcA);

            Assert.Equal(funcA, funcACopy);
        }

        [Fact]
        public void Functions_with_different_parameters_are_not_equal()
        {
            var parameters1 = Params(DataType("T1"), DataType("T2"));
            var funcA1 = Func("A", @params: parameters1);
            var parameters2 = Params(DataType("T1"), DataType("T3"));
            var funcA2 = Func(funcA1, @params: parameters2);

            Assert.NotEqual(funcA1, funcA2);
        }

        [Fact]
        public void Functions_with_different_return_types_are_not_equal()
        {
            var funcA1 = Func("A", @return: DataType("T1"));
            var funcA2 = Func(funcA1, @return: DataType("T2"));

            Assert.NotEqual(funcA1, funcA2);
        }

        [Fact]
        public void Is_not_equal_to_equivalent_method()
        {
            // Note that methods should really have different names than functions,
            // but, just in case, we need to check method vs. function in equality.
            var ns = Namespace();
            var parameters = Params(DataType("T1"), DataType("T2"));
            var funcA = Func("A", ns, parameters, DataType("T3"));
            var selfDataType = DataType("Class");
            var selfType = Type(ns, selfDataType);
            var methodA = Method("A", selfType, selfDataType, parameters, DataType("T3"));

            // Note: assert false used to ensure which object Equals is called on
            Assert.False(funcA.Equals(methodA));
        }
    }
}
