using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    [Trait("Category", "Symbols")]
    public class NamespaceSymbolTests : SymbolTestFixture
    {
        [Fact]
        public void Namespaces_with_same_name_and_containing_symbol_are_equal()
        {
            var container = Package("my.something_package");
            var nsFoo1 = Namespace("foo", container);
            var nsFoo2 = Namespace("foo", container);

            Assert.Equal(nsFoo1, nsFoo2);
        }

        [Fact]
        public void Namespaces_with_different_name_are_not_equal()
        {
            var container = Package("my.something_package");
            var nsFoo = Namespace("foo", container);
            var nsBar = Namespace("bar", container);

            Assert.NotEqual(nsFoo, nsBar);
        }

        [Fact]
        public void Namespaces_with_different_containing_symbols_are_not_equal()
        {
            var container1 = Package("my.something_package");
            var ns1 = Namespace("foo", container1);
            var container2 = Package("my.other_package");
            var ns2 = Namespace("foo", container2);

            Assert.NotEqual(ns1, ns2);
        }
    }
}
