using System;
using Adamant.Tools.Compiler.Bootstrap.Emit.C;

namespace UnitTests.Emit.C.Fakes
{
    public class FakeEmitter<T> : IEmitter<T>
    {
        public void Emit(T value, Code code)
        {
            throw new NotImplementedException();
        }
    }
}
