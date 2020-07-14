using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Types
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
        public void Is_constant()
        {
            var type = new IntegerConstantType(1);

            Assert.True(type.IsConstant);
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

        [Fact]
        public void Has_copy_semantics()
        {
            var type = new IntegerConstantType(1);

            Assert.Equal(TypeSemantics.Copy, type.Semantics);
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

        [Fact]
        public void Converts_to_non_constant_int_type()
        {
            // TODO larger values need to convert to larger integer types

            var type = new IntegerConstantType(42);

            var nonConstant = type.ToNonConstantType();

            Assert.Same(DataType.Int, nonConstant);
        }
    }
}
