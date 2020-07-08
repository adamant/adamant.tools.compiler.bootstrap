using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Metadata.Types
{
    [Trait("Category", "Types")]
    public class IntegerConstantTypeTests
    {
        [Fact]
        public void Is_integer_numeric_simple_value_type()
        {
            var type = new IntegerConstantType(1);

            Assert.OfType<IntegerType>(type);
            Assert.OfType<NumericType>(type);
            Assert.OfType<SimpleType>(type);
            Assert.OfType<ValueType>(type);
        }

        [Fact]
        public void Is_known_type()
        {
            var type = new IntegerConstantType(1);

            Assert.True(type.IsKnown);
        }

        [Fact]
        public void Is_not_empty_type()
        {
            var type = new IntegerConstantType(1);

            Assert.False(type.IsEmpty);
        }

        [Theory]
        [InlineData(-23)]
        [InlineData(-1)]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(42)]
        [InlineData(234_324_234_325)]
        public void Has_integer_value(long value)
        {
            var type = new IntegerConstantType(value);

            Assert.Equal(value, type.Value);
        }
    }
}
