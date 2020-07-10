using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Metadata.Types
{
    [Trait("Category", "Types")]
    public class ObjectTypeTests
    {
        [Fact]
        public void Has_reference_semantics()
        {
            var type = new ObjectType(Name.From("Foo", "Bar"), true, ReferenceCapability.Isolated);

            Assert.Equal(TypeSemantics.Reference, type.Semantics);
        }

        [Fact]
        public void Convert_to_non_constant_type_is_same_type()
        {
            var type = new ObjectType(Name.From("Foo", "Bar"), true, ReferenceCapability.Isolated);

            var nonConstant = type.ToNonConstantType();

            Assert.Same(type, nonConstant);
        }
    }
}
