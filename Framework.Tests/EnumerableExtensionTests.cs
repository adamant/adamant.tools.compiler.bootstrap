using System;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Framework.Tests
{
    public class EnumerableExtensionTests
    {
        [Fact]
        public void ZipWithoutResultSelectorCreatesTuples()
        {
            var items1 = new[] { 1, 2, 3 };
            var items2 = new[] { "1", "2", "3" };
            var expected = new[] { Tuple.Create(1, "1"), Tuple.Create(2, "2"), Tuple.Create(3, "3") };
            Assert.Equal(expected, items1.Zip(items2));
        }

        [Fact]
        public void CrossJoinCreatesEveryCombination()
        {
            var items1 = new[] { 1, 2, 3 };
            var items2 = new[] { "1", "2", "3" };
            var expected = new[]
            {
                Tuple.Create(1, "1"),
                Tuple.Create(1, "2"),
                Tuple.Create(1, "3"),
                Tuple.Create(2, "1"),
                Tuple.Create(2, "2"),
                Tuple.Create(2, "3"),
                Tuple.Create(3, "1"),
                Tuple.Create(3, "2"),
                Tuple.Create(3, "3"),
            }.ToHashSet();
            Assert.Equal(expected, items1.CrossJoin(items2, (x, y) => Tuple.Create(x, y)).ToHashSet());
        }
    }
}
