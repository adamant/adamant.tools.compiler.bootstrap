using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Types
{
    [Trait("Category", "Types")]
    public class VoidTypeTests
    {
        [Fact]
        public void Is_empty_data_type()
        {
            var type = VoidType.Instance;

            Assert.OfType<EmptyType>(type);
        }

        [Fact]
        public void Is_known_type()
        {
            var type = VoidType.Instance;

            Assert.True(type.IsKnown);
        }

        [Fact]
        public void Is_empty_type()
        {
            var type = VoidType.Instance;

            Assert.True(type.IsEmpty);
        }

        [Fact]
        public void Void_has_void_semantics()
        {
            var type = VoidType.Instance;

            Assert.Equal(TypeSemantics.Void, type.Semantics);
        }

        [Fact]
        public void Has_special_name_never()
        {
            var type = VoidType.Instance;

            Assert.Equal(SpecialNames.Void, type.Name);
        }

        [Fact]
        public void Has_proper_ToString()
        {
            var type = VoidType.Instance;

            Assert.Equal("void", type.ToString());
        }

        [Fact]
        public void Convert_to_read_only_has_no_effect()
        {
            var type = NeverType.Instance;

            var @readonly = type.ToReadOnly();

            Assert.Equal(type, @readonly);
        }


        [Fact]
        public void Equal_to_itself()
        {
            Assert.Equal(NeverType.Instance, NeverType.Instance);
        }
    }
}
