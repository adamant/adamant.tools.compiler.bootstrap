using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;
using static Adamant.Tools.Compiler.Bootstrap.Types.ReferenceCapability;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Types
{
    [Trait("Category", "Types")]
    public class ObjectTypeTests
    {
        [Fact]
        public void Has_reference_semantics()
        {
            var type = new ObjectType(MaybeQualifiedName.From("Foo", "Bar"), true, Isolated);

            Assert.Equal(TypeSemantics.Reference, type.Semantics);
        }

        [Fact]
        public void Convert_to_non_constant_type_is_same_type()
        {
            var type = new ObjectType(MaybeQualifiedName.From("Foo", "Bar"), true, Isolated);

            var nonConstant = type.ToNonConstantType();

            Assert.Same(type, nonConstant);
        }

        [Fact]
        public void With_same_name_mutability_and_reference_capability_are_equal()
        {
            var type1 = new ObjectType(MaybeQualifiedName.From("Foo", "Bar"), true, Isolated);
            var type2 = new ObjectType(MaybeQualifiedName.From("Foo", "Bar"), true, Isolated);

            Assert.Equal(type1, type2);
        }
    }
}
