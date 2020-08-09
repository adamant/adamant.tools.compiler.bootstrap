using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Types
{
    [Trait("Category", "Types")]
    public class BoolConstantTypeTests
    {
        [Fact]
        public void Is_a_boolean_type()
        {
            var type = BoolConstantType.True;

            Assert.OfType<BoolType>(type);
        }

        [Fact]
        public void Is_constant()
        {
            var type = BoolConstantType.True;

            Assert.True(type.IsConstant);
        }

        [Fact]
        public void True_type_has_true_value()
        {
            var type = BoolConstantType.True;

            Assert.True(type.Value);
        }

        [Fact]
        public void False_type_has_false_value()
        {
            var type = BoolConstantType.False;

            Assert.False(type.Value);
        }

        [Fact]
        public void True_has_special_name_and_ToString()
        {
            var type = BoolConstantType.True;

            Assert.Equal(SpecialTypeName.True, type.Name);
            Assert.Equal("const[true]", type.ToString());
        }

        [Fact]
        public void False_has_special_name_and_ToString()
        {
            var type = BoolConstantType.False;

            Assert.Equal(SpecialTypeName.False, type.Name);
            Assert.Equal("const[false]", type.ToString());
        }

        [Fact]
        public void Converts_to_non_constant_bool_type()
        {
            var type = BoolConstantType.False;

            var nonConstant = type.ToNonConstantType();

            Assert.Same(DataType.Bool, nonConstant);
        }

        [Fact]
        public void Bool_constant_types_with_same_value_are_equal()
        {
            Assert.Equal(BoolConstantType.True, BoolConstantType.True);
            Assert.Equal(BoolConstantType.False, BoolConstantType.False);
        }

        [Fact]
        public void Any_types_with_different_reference_capabilities_are_not_equal()
        {
            Assert.NotEqual(BoolConstantType.True, BoolConstantType.False);
        }
    }
}
