using Adamant.Tools.Compiler.Bootstrap.Framework;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Framework
{
    [UnitTest]
    public class YieldTests
    {
        [Fact]
        public void YeildIsSingleItem()
        {
            var x = new object();
            Assert.Single(x.Yield(), x);
        }

        [Fact]
        public void YieldOfNullIsSingleItem()
        {
            object x = null;
            Assert.Single(x.Yield(), x);
        }

        [Fact]
        public void YieldValueOfClassIsSingleItem()
        {
            var x = new object();
            Assert.Single(x.YieldValue(), x);
        }

        [Fact]
        public void YieldValueOfNullClassIsEmpty()
        {
            object x = null;
            Assert.Empty(x.YieldValue());
        }

        [Fact]
        public void YieldValueOfValueTypeIsSingleItem()
        {
            int? x = 42;
            Assert.Single(x.YieldValue(), x);
        }

        [Fact]
        public void YieldValueOfNullValueTypeIsEmpty()
        {
            int? x = null;
            Assert.Empty(x.YieldValue());
        }
    }
}
