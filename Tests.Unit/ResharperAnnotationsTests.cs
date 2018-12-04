using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;
using Xunit;
using Xunit.Categories;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit
{
    [UnitTest]
    public class ResharperAnnotationsTests
    {
        [Fact]
        public void Item_not_null_enumerable()
        {
            var _ = ItemNotNullEnumerable().Select(x => x.ToString()).ToList();
        }

        [Fact]
        public void Item_can_be_null_enumerable()
        {
            var _ = ItemCanBeNullEnumerable().Select(x => x.ToString()).ToList();
        }

        [NotNull]
        [ItemNotNull]
        private static IEnumerable<object> ItemNotNullEnumerable()
        {
            yield break;
        }

        [NotNull]
        [ItemCanBeNull]
        private static IEnumerable<object> ItemCanBeNullEnumerable()
        {
            yield break;
        }
    }
}
