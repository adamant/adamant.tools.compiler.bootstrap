using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Symbols
{
    public class NamespaceSymbolTests
    {
        [Fact]
        public void Namespaces_with_same_name_are_equal()
        {
            var ns1 = new NamespaceSymbol(Name.From("foo", "bar", "baz"), SymbolSet.Empty);
            var ns2 = new NamespaceSymbol(Name.From("foo", "bar", "baz"), SymbolSet.Empty);

            Assert.Equal(ns1, ns2);
        }

        [Fact]
        public void Namespaces_with_different_name_are_not_equal()
        {
            var ns1 = new NamespaceSymbol(Name.From("foo", "bar", "baz"), SymbolSet.Empty);
            var ns2 = new NamespaceSymbol(Name.From("foo", "bar", "biff"), SymbolSet.Empty);

            Assert.NotEqual(ns1, ns2);
        }
    }
}
