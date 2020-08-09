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
            var type = PointerSizedIntegerType.Size;

            Assert.False(type.IsSigned);
        }

        [Fact]
        public void Size_is_known_type()
        {
            var type = PointerSizedIntegerType.Size;

            Assert.True(type.IsKnown);
        }

        [Fact]
        public void Size_has_copy_semantics()
        {
            var type = PointerSizedIntegerType.Size;

            Assert.Equal(TypeSemantics.Copy, type.Semantics);
        }

        [Fact]
        public void Offset_is_signed()
        {
            var type = PointerSizedIntegerType.Offset;

            Assert.True(type.IsSigned);
        }

        [Fact]
        public void Offset_is_known_type()
        {
            var type = PointerSizedIntegerType.Offset;

            Assert.True(type.IsKnown);
        }

        [Fact]
        public void Offset_has_copy_semantics()
        {
            var type = PointerSizedIntegerType.Offset;

            Assert.Equal(TypeSemantics.Copy, type.Semantics);
        }

        [Fact]
        public void Types_equal_to_themselves_and_not_others()
        {
            Assert.Equal(PointerSizedIntegerType.Size, PointerSizedIntegerType.Size);
            Assert.Equal(PointerSizedIntegerType.Offset, PointerSizedIntegerType.Offset);

            Assert.NotEqual(PointerSizedIntegerType.Size, PointerSizedIntegerType.Offset);
        }
    }
}
