using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Types
{
    [Trait("Category", "Types")]
    public class UnsizedIntegerTypeTests
    {
        [Fact]
        public void Size_is_unsigned()
        {
            var type = UnsizedIntegerType.Size;

            Assert.False(type.IsSigned);
        }

        [Fact]
        public void Size_is_known_type()
        {
            var type = UnsizedIntegerType.Size;

            Assert.True(type.IsKnown);
        }

        [Fact]
        public void Size_has_copy_semantics()
        {
            var type = UnsizedIntegerType.Size;

            Assert.Equal(TypeSemantics.Copy, type.Semantics);
        }

        [Fact]
        public void Offset_is_signed()
        {
            var type = UnsizedIntegerType.Offset;

            Assert.True(type.IsSigned);
        }

        [Fact]
        public void Offset_is_known_type()
        {
            var type = UnsizedIntegerType.Offset;

            Assert.True(type.IsKnown);
        }

        [Fact]
        public void Offset_has_copy_semantics()
        {
            var type = UnsizedIntegerType.Offset;

            Assert.Equal(TypeSemantics.Copy, type.Semantics);
        }
    }
}
