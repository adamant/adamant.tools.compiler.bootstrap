using System;
using Adamant.Tools.Compiler.Bootstrap.Emit.C;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Emit.C.Fakes
{
    public class FakeEmitter<T> : IEmitter<T>
    {
        public void Emit(T value, Code code)
        {
            throw new NotImplementedException();
        }
    }
}
