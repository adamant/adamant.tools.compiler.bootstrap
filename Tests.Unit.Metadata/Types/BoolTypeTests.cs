using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Metadata.Types
{
    [Trait("Category", "Types")]
    public class BoolTypeTests
    {
        [Fact]
        public void Is_simple_value_type()
        {
            var type = BoolType.Instance;

            Assert.OfType<SimpleType>(type);
            Assert.OfType<ValueType>(type);
        }

        [Fact]
        public void Is_known_type()
        {
            var type = BoolType.Instance;

            Assert.True(type.IsKnown);
        }

        [Fact]
        public void Is_not_empty_type()
        {
            var type = BoolType.Instance;

            Assert.False(type.IsEmpty);
        }

        [Fact]
        public void Has_copy_semantics()
        {
            var type = BoolType.Instance;

            Assert.Equal(ValueSemantics.Copy, type.ValueSemantics);
        }

        [Fact]
        public void Has_special_name_bool()
        {
            var type = BoolType.Instance;

            Assert.Equal(SpecialName.Bool, type.Name);
        }

        [Fact]
        public void Has_proper_ToString()
        {
            var type = BoolType.Instance;

            Assert.Equal("bool", type.ToString());
        }

        [Fact]
        public void Convert_to_read_only_has_no_effect()
        {
            var type = BoolType.Instance;

            var @readonly = type.ToReadOnly();

            Assert.Equal(type, @readonly);
        }
    }
}
