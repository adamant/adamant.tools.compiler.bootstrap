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
            var type = SizedIntegerType.Byte;

            Assert.Equal(8, type.Bits);
        }

        [Fact]
        public void Byte_is_unsigned()
        {
            var type = SizedIntegerType.Byte;

            Assert.False(type.IsSigned);
        }

        [Fact]
        public void Int_has_copy_semantics()
        {
            var type = SizedIntegerType.Int;

            Assert.Equal(TypeSemantics.Copy, type.Semantics);
        }

        [Fact]
        public void Types_equal_to_themselves_and_not_others()
        {
            Assert.Equal(SizedIntegerType.Int, SizedIntegerType.Int);
            Assert.Equal(SizedIntegerType.UInt, SizedIntegerType.UInt);
            Assert.Equal(SizedIntegerType.Byte, SizedIntegerType.Byte);

            Assert.NotEqual(SizedIntegerType.Int, SizedIntegerType.UInt);
            Assert.NotEqual(SizedIntegerType.Int, SizedIntegerType.Byte);
        }
    }
}
