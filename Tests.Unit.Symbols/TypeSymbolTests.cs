using System;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    [Trait("Category", "Symbols")]
    public class TypeSymbolTests
    {
        [Fact]
        public void Type_declared_must_match_symbol_name()
        {
            var type = new ObjectType(Name.From("foo", "bar", "My_Class"), true, ReferenceCapability.Isolated);

            Assert.Throws<ArgumentException>(() => new TypeSymbol(Name.From("foo", "bar", "Different_name"), type));
        }

        [Fact]
        public void With_same_name_and_type_are_equal()
        {
            var type = new ObjectType(Name.From("foo", "bar", "My_Class"), true, ReferenceCapability.Isolated);
            var sym1 = new TypeSymbol(Name.From("foo", "bar", "My_Class"), type);
            var sym2 = new TypeSymbol(Name.From("foo", "bar", "My_Class"), type);

            Assert.Equal(sym1, sym2);
        }

        [Fact]
        public void With_different_name_are_not_equal()
        {
            var type1 = new ObjectType(Name.From("foo", "bar", "My_Class1"), true, ReferenceCapability.Isolated);
            var sym1 = new TypeSymbol(Name.From("foo", "bar", "My_Class1"), type1);
            var type2 = new ObjectType(Name.From("foo", "bar", "My_Class2"), true, ReferenceCapability.Isolated);
            var sym2 = new TypeSymbol(Name.From("foo", "bar", "My_Class2"), type2);

            Assert.NotEqual(sym1, sym2);
        }

        [Fact]
        public void With_different_type_are_not_equal()
        {
            var type1 = new ObjectType(Name.From("foo", "bar", "My_Class"), true, ReferenceCapability.Isolated);
            var sym1 = new TypeSymbol(Name.From("foo", "bar", "My_Class"), type1);
            var type2 = new ObjectType(Name.From("foo", "bar", "My_Class"), false, ReferenceCapability.Isolated);
            var sym2 = new TypeSymbol(Name.From("foo", "bar", "My_Class"), type2);

            Assert.NotEqual(sym1, sym2);
        }
    }
}
