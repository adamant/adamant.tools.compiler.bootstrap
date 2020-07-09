using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Metadata.Types
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

            Assert.Equal(ValueSemantics.Copy, type.ValueSemantics);
        }
    }
}
