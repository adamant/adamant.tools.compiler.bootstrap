using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    public class BindingSymbolTests
    {
        [Fact]
        public void Has_properties_constructed_with()
        {
            var name = Name.From("foo", "bar", "Baz");
            var type = new ObjectType(Name.From("Fake"), true, ReferenceCapability.Borrowed);

            var symbol = new BindingSymbol(name, true, type);

            Assert.Equal(name, symbol.FullName);
            Assert.True(symbol.IsMutableBinding);
            Assert.Equal(type, symbol.Type);
        }

        // TODO Bindings with the same name should match mutability and type
    }
}
