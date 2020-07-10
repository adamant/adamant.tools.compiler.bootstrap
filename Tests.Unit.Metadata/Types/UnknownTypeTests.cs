using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Metadata.Types
{
    [Trait("Category", "Types")]
    public class UnknownTypeTests
    {
        [Fact]
        public void Unknown_is_NOT_a_known_type()
        {
            var type = UnknownType.Instance;

            Assert.False(type.IsKnown);
        }

        [Fact]
        public void Is_not_constant()
        {
            var type = UnknownType.Instance;

            Assert.False(type.IsConstant);
        }

        /// <summary>
        /// It has never semantics because it is assignable to anything
        /// </summary>
        [Fact]
        public void Unknown_has_never_semantics()
        {
            var type = UnknownType.Instance;

            Assert.Equal(TypeSemantics.Never, type.Semantics);
        }
    }
}
