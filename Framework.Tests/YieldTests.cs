using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Framework.Tests
{
    public class YieldTests
    {
        [Fact]
        [UnitTest]
        public void YeildIsSingleItem()
        {
            var x = new object();
            Assert.Single(x.Yield(), x);
        }

        [Fact]
        [UnitTest]
        public void YieldOfNullIsSingleItem()
        {
            object x = null;
            Assert.Single(x.Yield(), x);
        }

        [Fact]
        [UnitTest]
        public void YieldValueOfClassIsSingleItem()
        {
            var x = new object();
            Assert.Single(x.YieldValue(), x);
        }

        [Fact]
        [UnitTest]
        public void YieldValueOfNullClassIsEmpty()
        {
            object x = null;
            Assert.Empty(x.YieldValue());
        }

        [Fact]
        [UnitTest]
        public void YieldValueOfValueTypeIsSingleItem()
        {
            int? x = 42;
            Assert.Single(x.YieldValue(), x);
        }

        [Fact]
        [UnitTest]
        public void YieldValueOfNullValueTypeIsEmpty()
        {
            int? x = null;
            Assert.Empty(x.YieldValue());
        }
    }
}
