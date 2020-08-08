using Adamant.Tools.Compiler.Bootstrap.Framework;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Framework
{
    [Trait("Category", "Framework")]
    public class YieldTests
    {
        [Fact]
        public void YieldIsSingleItem()
        {
            var x = new object();
            Assert.Single(x.Yield(), x);
        }

        [Fact]
        public void YieldOfNullIsSingleItem()
        {
            object? x = null;
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
            object? x = null;
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
