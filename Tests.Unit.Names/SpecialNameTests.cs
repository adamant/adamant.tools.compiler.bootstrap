using Adamant.Tools.Compiler.Bootstrap.Names;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Names
{
    [Trait("Category", "Names")]
    public class SpecialNameTests
    {
        [Fact]
        public void Constructor_is_special()
        {
            var name = SpecialNames.Constructor("from_foo");

            Assert.True(name.IsSpecial);
        }

        [Fact]
        public void Constructor_name_is_prefixed_with_new()
        {
            var name = SpecialNames.Constructor("from_foo");

            Assert.Equal("new_from_foo", name.Text);
        }

        [Fact]
        public void Constructor_without_name_is_just_new()
        {
            var name = SpecialNames.Constructor();

            Assert.Equal("new", name.Text);
        }
    }
}
