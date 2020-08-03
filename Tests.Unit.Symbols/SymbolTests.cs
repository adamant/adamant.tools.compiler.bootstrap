using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Moq;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    [Trait("Category", "Symbols")]
    public class SymbolTests : SymbolTestFixture
    {
        [Fact]
        public void Symbol_not_in_namespace_is_global()
        {
            var symbol = FakeSymbol(null, Name("My_Class"));

            Assert.True(symbol.IsGlobal);
        }

        [Fact]
        public void Symbol_in_namespace_is_not_global()
        {
            var symbol = FakeSymbol(Namespace(), Name("My_Class"));

            Assert.False(symbol.IsGlobal);
        }

        private static Symbol FakeSymbol(NamespaceOrPackageSymbol? containing, SimpleName name)
        {
            return new Mock<Symbol>(MockBehavior.Default, containing, name).Object;
        }
    }
}
