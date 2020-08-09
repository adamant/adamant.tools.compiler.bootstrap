using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Types
{
    [Trait("Category", "Types")]
    public class SizedIntegerTypeTests
    {
        [Fact]
        public void Byte_has_8_bits()
        {
            var type = FixedSizeIntegerType.Byte;

            Assert.Equal(8, type.Bits);
        }

        [Fact]
        public void Byte_is_unsigned()
        {
            var type = FixedSizeIntegerType.Byte;

            Assert.False(type.IsSigned);
        }

        [Fact]
        public void Int_has_copy_semantics()
        {
            var type = FixedSizeIntegerType.Int;

            Assert.Equal(TypeSemantics.Copy, type.Semantics);
        }

        [Fact]
        public void Types_equal_to_themselves_and_not_others()
        {
            Assert.Equal(FixedSizeIntegerType.Int, FixedSizeIntegerType.Int);
            Assert.Equal(FixedSizeIntegerType.UInt, FixedSizeIntegerType.UInt);
            Assert.Equal(FixedSizeIntegerType.Byte, FixedSizeIntegerType.Byte);

            Assert.NotEqual(FixedSizeIntegerType.Int, FixedSizeIntegerType.UInt);
            Assert.NotEqual(FixedSizeIntegerType.Int, FixedSizeIntegerType.Byte);
        }
    }
}
