using System;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    [Trait("Category", "Symbols")]
    public class TypeSymbolTests : SymbolTestFixture
    {
        [Fact]
        public void Type_declared_must_match_symbol_name()
        {
            Assert.Throws<ArgumentException>(() => Type("T1", dataType: DataType("T2")));
        }

        [Fact]
        public void With_same_name_and_type_are_equal()
        {
            var container = Package("my.package");
            var type = DataType("T1");
            var sym1 = Type("T1", container, type);
            var sym2 = Type("T1", container, type);

            Assert.Equal(sym1, sym2);
        }

        [Fact]
        public void With_different_name_are_not_equal()
        {
            var container = Package("my.package");
            var type1 = DataType("My_Class1");
            var sym1 = Type("My_Class1", container, type1);
            var type2 = DataType("My_Class2");
            var sym2 = Type("My_Class2", container, type2);

            Assert.NotEqual(sym1, sym2);
        }

        [Fact]
        public void With_different_type_are_not_equal()
        {
            var container = new PackageSymbol(Name("my.package"));
            var type1 = DataType("T1");
            var sym1 = Type("T1", container, type1);
            var type2 = DataType("T2");
            var sym2 = Type("T2", container, type2);

            Assert.NotEqual(sym1, sym2);
        }
    }
}
