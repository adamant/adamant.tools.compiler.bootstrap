using System;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Names
{
    [Trait("Category", "Names")]
    public class NamespaceNameTests
    {
        [Fact]
        public void Name_qualified_with_global_namespace_is_unchanged()
        {
            var ns = Namespace("foo", "bar", "baz");

            var qualified = NamespaceName.Global.Qualify(ns);

            Assert.Equal(ns, qualified);
        }

        [Fact]
        public void Qualified_name_combines_segments()
        {
            var ns1 = Namespace("foo", "bar", "baz");
            var ns2 = Namespace("qux", "quux", "quuz");

            var qualified = ns1.Qualify(ns2);

            Assert.Equal(Namespace("foo", "bar", "baz", "qux", "quux", "quuz"), qualified);
        }

        [Fact]
        public void Cannot_qualify_global_namespace()
        {
            var ns = Namespace("foo", "bar", "baz");

            Assert.Throws<ArgumentException>(() => ns.Qualify(NamespaceName.Global));
        }

        [Fact]
        public void Namespace_names_of_typical_namespace()
        {
            var ns = Namespace("foo", "bar", "baz");

            var names = ns.NamespaceNames();

            var expected = new[]
            {
                Namespace("foo"), Namespace("foo", "bar"), Namespace("foo", "bar", "baz")
            };
            Assert.Equal(expected, names);
        }

        [Fact]
        public void Namespace_names_of_global_namespace()
        {
            var ns = Namespace();

            var names = ns.NamespaceNames();

            Assert.Empty(names);
        }

        private static NamespaceName Namespace(params string[] segments)
        {
            return new NamespaceName(segments.Select(s => new Name(s)));
        }
    }
}
