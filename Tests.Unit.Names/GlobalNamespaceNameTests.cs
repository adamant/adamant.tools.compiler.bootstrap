using Adamant.Tools.Compiler.Bootstrap.Names;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Names
{
    [Trait("Category", "Names")]
    public class GlobalNamespaceNameTests
    {
        [Fact]
        public void Not_in_any_namespaces()
        {
            var globalNamespace = GlobalNamespaceName.Instance;

            Assert.Empty(globalNamespace.NamespaceNames());
            Assert.Empty(globalNamespace.NestedInNames());
        }

        [Fact]
        public void ToString_is_double_colon()
        {
            var globalNamespace = GlobalNamespaceName.Instance;

            var str = globalNamespace.ToString();

            Assert.Equal("::", str);
        }

        [Fact]
        public void Qualifying_name_with_global_namespace_returns_same_name()
        {
            var globalNamespace = GlobalNamespaceName.Instance;
            var name = MaybeQualifiedName.From("foo", "bar", "My_Class");

            var qualifiedName = globalNamespace.Qualify(name);

            Assert.Same(name, qualifiedName);
        }
    }
}
