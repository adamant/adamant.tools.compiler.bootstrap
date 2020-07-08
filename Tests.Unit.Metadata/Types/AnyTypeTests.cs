using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Metadata.Types
{
    [Trait("Category", "Types")]
    public class AnyTypeTests
    {
        [Fact]
        public void Is_reference_type()
        {
            var type = new AnyType(ReferenceCapability.Isolated);

            Assert.OfType<ReferenceType>(type);
        }
    }
}
