using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Types
{
    [Trait("Category", "Types")]
    public class DataTypeTests
    {
        [Fact]
        public void None_type_is_optional_never()
        {
            var none = DataType.None;

            Assert.OfType<OptionalType>(none);
            Assert.Equal(NeverType.Instance, none.Referent);
        }
    }
}
