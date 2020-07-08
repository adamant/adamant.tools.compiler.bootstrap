using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Xunit;
using static Adamant.Tools.Compiler.Bootstrap.Metadata.Types.ReferenceCapability;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Metadata.Types
{
    [Trait("Category", "Types")]
    public class AnyTypeTests
    {
        [Fact]
        public void Is_reference_type()
        {
            var type = new AnyType(Isolated);

            Assert.OfType<ReferenceType>(type);
        }

        [Fact]
        public void Is_known_type()
        {
            var type = new AnyType(Isolated);

            Assert.True(type.IsKnown);
        }

        [Fact]
        public void Is_not_empty_type()
        {
            var type = new AnyType(Isolated);

            Assert.False(type.IsEmpty);
        }

        [Fact]
        public void Is_declared_mutable()
        {
            var type = new AnyType(Isolated);

            Assert.True(type.DeclaredMutable);
        }

        [Theory]
        [InlineData(Isolated, "iso Any")]
        [InlineData(OwnedMutable, "owned mut Any")]
        [InlineData(Borrowed, "mut Any")]
        [InlineData(Shared, "Any")]
        public void ToString_includes_reference_capability(ReferenceCapability capability, string expected)
        {
            var type = new AnyType(capability);

            Assert.Equal(expected, type.ToString());
        }

        [Theory]
        [InlineData(Isolated)]
        [InlineData(OwnedMutable)]
        [InlineData(Borrowed)]
        [InlineData(Shared)]
        public void Has_reference_capability_constructed_with(ReferenceCapability capability)
        {
            var type = new AnyType(capability);

            Assert.Equal(capability, type.ReferenceCapability);
        }

        [Theory]
        [InlineData(Isolated)]
        [InlineData(OwnedMutable)]
        [InlineData(Borrowed)]
        [InlineData(Shared)]
        public void Can_convert_to_reference_capability(ReferenceCapability capability)
        {
            var type = new AnyType(Isolated);

            var converted = type.To(capability);

            Assert.Equal(capability, converted.ReferenceCapability);
        }
    }
}
