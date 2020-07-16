using Adamant.Tools.Compiler.Bootstrap.Names;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Names
{
    public class SpecialNameTests
    {
        [Fact]
        public void Constructor_is_special()
        {
            var name = SpecialName.Constructor("from_foo");

            Assert.True(name.IsSpecial);
        }

        [Fact]
        public void Constructor_name_is_prefixed_with_new()
        {
            var name = SpecialName.Constructor("from_foo");

            Assert.Equal("new_from_foo", name.Text);
        }

        [Fact]
        public void Constructor_without_name_is_just_new()
        {
            var name = SpecialName.Constructor();

            Assert.Equal("new", name.Text);
        }
    }
}
