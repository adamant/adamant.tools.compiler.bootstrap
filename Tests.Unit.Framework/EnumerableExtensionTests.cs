using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Framework
{
    [Trait("Category", "Core")]
    public class EnumerableExtensionTests
    {
        [Fact]
        public void CrossJoinCreatesEveryCombination()
        {
            var items1 = new[] { 1, 2, 3 };
            var items2 = new[] { "1", "2", "3" };
            var expected = new[]
            {
                (1, "1"),
                (1, "2"),
                (1, "3"),
                (2, "1"),
                (2, "2"),
                (2, "3"),
                (3, "1"),
                (3, "2"),
                (3, "3"),
            }.ToHashSet();
            Assert.Equal(expected, items1.CrossJoin(items2).ToHashSet());
        }
    }
}
