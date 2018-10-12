using System;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Emit.C.Fakes
{
    public class FakeDataType : DataType
    {
        private readonly Guid instance;

        public FakeDataType()
        {
            instance = Guid.NewGuid();
        }

        public override string ToString()
        {
            return $"⧼Fake Type {instance}⧽";
        }
    }
}
