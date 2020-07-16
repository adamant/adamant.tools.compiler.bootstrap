using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    public class SymbolSetTests
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
            var sym1 = new BindingSymbol(Name.From("foo"), true, DataType.Int);
            var sym1Dup = new BindingSymbol(Name.From("foo"), true, DataType.Int);
            var sym2 = new BindingSymbol(Name.From("bar"), false, DataType.Bool);
            var sym3 = new BindingSymbol(Name.From("baz"), false, new ObjectType(Name.From("My_Class"), true, ReferenceCapability.Borrowed));

            var set = new SymbolSet(new[] { sym1, sym1Dup, sym2, sym2, sym3 });

            Assert.Equal(new[] { sym1, sym2, sym3 }, set);
        }
    }
}
