using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Metadata.Types
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
        public void True_has_special_name_bool_an_ToString()
        {
            var type = BoolConstantType.True;

            Assert.Equal(SimpleName.Special("const[true]"), type.Name);
            Assert.Equal("const[true]", type.ToString());
        }

        [Fact]
        public void False_has_special_name_bool_an_ToString()
        {
            var type = BoolConstantType.False;

            Assert.Equal(SimpleName.Special("const[false]"), type.Name);
            Assert.Equal("const[false]", type.ToString());
        }
    }
}
