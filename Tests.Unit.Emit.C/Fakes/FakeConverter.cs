using Adamant.Tools.Compiler.Bootstrap.Emit.C;
using JetBrains.Annotations;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Emit.C.Fakes
{
    public class FakeConverter<T> : IConverter<T>
    {
        public string Convert([NotNull] T value)
        {
            Assert.NotNull(value);
            return value.ToString();
        }
    }
}
