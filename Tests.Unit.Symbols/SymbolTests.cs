using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Moq;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    [Trait("Category", "Symbols")]
    public class SymbolTests
    {
        [Fact]
        public void Symbol_not_in_namespace_is_global()
        {
            var symbol = FakeSymbol(Name.From("My_Class"));

            Assert.True(symbol.IsGlobal);
        }

        [Fact]
        public void Symbol_in_namespace_is_not_global()
        {
            var symbol = FakeSymbol(Name.From("ns", "My_Class"));

            Assert.False(symbol.IsGlobal);
        }

        private static Symbol FakeSymbol(Name name)
        {
            return new Mock<Symbol>(MockBehavior.Default, name).Object;
        }
    }
}
