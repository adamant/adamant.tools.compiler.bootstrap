using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    [Trait("Category", "Symbols")]
    public class MethodSymbolTests : SymbolTestFixture
    {
        [Fact]
        public void Methods_with_same_name_parameters_and_return_type_are_equal()
        {
            var methodA = Method("A");
            var methodACopy = Method(methodA);

            Assert.Equal(methodA, methodACopy);
        }

        [Fact]
        public void Methods_with_different_self_parameters_are_not_equal()
        {
            var selfType1 = DataType("T1");
            var methodA1 = Method("A", self: selfType1);
            var selfType2 = DataType("T2");
            var methodA2 = Method(methodA1, self: selfType2);

            Assert.NotEqual(methodA1, methodA2);
        }

        [Fact]
        public void Methods_with_different_parameters_are_not_equal()
        {
            var parameters1 = Params(DataType("T1"), DataType("T2"));
            var methodA1 = Method("A", @params: parameters1);
            var parameters2 = Params(DataType("T1"), DataType("T3"));
            var methodA2 = Method(methodA1, @params: parameters2);

            Assert.NotEqual(methodA1, methodA2);
        }

        [Fact]
        public void Methods_with_different_return_types_are_not_equal()
        {
            var methodA1 = Method("A", @return: DataType("T1"));
            var methodA2 = Method(methodA1, @return: DataType("T2"));

            Assert.NotEqual(methodA1, methodA2);
        }

        [Fact]
        public void Is_not_equal_to_equivalent_function()
        {
            // Note that methods should really have different names than functions,
            // but, just in case, we need to check method vs. function in equality.
            var ns = Namespace();
            var parameters = Params(DataType("T1"), DataType("T2"));
            var selfDataType = DataType("Class");
            var selfType = Type(ns, selfDataType);
            var method = Method("A", selfType, selfDataType, parameters, DataType("T3"));
            var func = Func("A", ns, parameters, DataType("T3"));

            // Note: assert false used to ensure which object Equals is called on
            Assert.False(method.Equals(func));
        }
    }
}
