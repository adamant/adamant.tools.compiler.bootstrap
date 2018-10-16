using Adamant.Tools.Compiler.Bootstrap.Emit.C;
using Xunit;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Emit.C.Fakes
{
    public class FakeEmitter<T> : IEmitter<T>
    {
        public void Emit(T value, Code code)
        {
            Assert.NotNull(value);
            // TODO should we actually write something to code?
        }
    }
}
