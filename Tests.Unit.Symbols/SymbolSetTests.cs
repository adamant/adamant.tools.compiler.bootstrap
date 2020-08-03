using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    [Trait("Category", "Symbols")]
    public class SymbolSetTests : SymbolTestFixture
    {
        [Fact]
        public void Empty_symbol_set_is_empty()
        {
            var empty = SymbolSet.Empty;

            Assert.Empty(empty);
        }

        [Fact]
        public void Contains_distinct_symbols()
        {
            var sym1 = Variable("sym1");
            var sym1Dup = Variable(sym1);
            var sym2 = Variable("sym2");
            var sym3 = Variable("sym3");

            var set = new SymbolSet(new[] { sym1, sym1Dup, sym2, sym2, sym3 });

            Assert.Equal(new[] { sym1, sym2, sym3 }, set);
        }
    }
}
