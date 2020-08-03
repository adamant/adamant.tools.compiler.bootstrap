using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    [Trait("Category", "Symbols")]
    public class VariableSymbolTests : SymbolTestFixture
    {
        [Fact]
        public void Has_properties_constructed_with()
        {
            var func = Func();
            var dataType = DataType();
            var symbol = Variable("foo", func, 42, true, dataType);

            Assert.Equal(func, symbol.ContainingSymbol);
            Assert.Equal(Name("foo"), symbol.Name);
            Assert.Equal(42, symbol.DeclarationNumber);
            Assert.True(symbol.IsMutableBinding);
            Assert.Equal(dataType, symbol.DataType);
        }

        [Fact]
        public void Variables_with_same_name_mutability_and_type_are_equal()
        {
            var varA = Variable("a");
            var varACopy = Variable(varA);

            Assert.Equal(varA, varACopy);
        }

        [Fact]
        public void Variables_with_different_mutability_are_not_equal()
        {
            var varA1 = Variable("a", mut: true);
            var varA2 = Variable(varA1, mut: false);

            Assert.NotEqual(varA1, varA2);
        }

        [Fact]
        public void Variables_with_different_types_are_not_equal()
        {
            var varA1 = Variable("a", type: DataType("T1"));
            var varA2 = Variable(varA1, type: DataType("T2"));

            Assert.NotEqual(varA1, varA2);
        }
    }
}
