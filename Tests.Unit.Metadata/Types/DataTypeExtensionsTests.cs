using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Metadata.Types
{
    public class DataTypeExtensionsTests
    {
        [Fact]
        public void Bool_constant_types_is_assignable_to_bool_type()
        {
            var trueAssignable = DataType.Bool.IsAssignableFrom(DataType.True);
            var falseAssignable = DataType.Bool.IsAssignableFrom(DataType.False);

            Assert.True(trueAssignable, $"{DataType.True} not assignable to {DataType.Bool}");
            Assert.True(falseAssignable, $"{DataType.False} not assignable to {DataType.Bool}");
        }

        // TODO integer constant type assignability
    }
}
