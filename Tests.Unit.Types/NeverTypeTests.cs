using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Types
{
    [Trait("Category", "Types")]
    public class NeverTypeTests
    {
        [Fact]
        public void Is_empty_data_type()
        {
            var type = NeverType.Instance;

            Assert.OfType<EmptyType>(type);
        }

        [Fact]
        public void Is_known_type()
        {
            var type = NeverType.Instance;

            Assert.True(type.IsKnown);
        }

        [Fact]
        public void Is_empty_type()
        {
            var type = NeverType.Instance;

            Assert.True(type.IsEmpty);
        }

        [Fact]
        public void Never_has_never_semantics()
        {
            var type = NeverType.Instance;

            Assert.Equal(TypeSemantics.Never, type.Semantics);
        }

        [Fact]
        public void Has_special_name_never()
        {
            var type = NeverType.Instance;

            Assert.Equal(SpecialName.Never, type.Name);
        }

        [Fact]
        public void Has_proper_ToString()
        {
            var type = NeverType.Instance;

            Assert.Equal("never", type.ToString());
        }

        [Fact]
        public void Convert_to_read_only_has_no_effect()
        {
            var type = NeverType.Instance;

            var @readonly = type.ToReadOnly();

            Assert.Equal(type, @readonly);
        }

        [Fact]
        public void Is_equal_to_itself()
        {
            Assert.Equal(NeverType.Instance, NeverType.Instance);
        }
    }
}
