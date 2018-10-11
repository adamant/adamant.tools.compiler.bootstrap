using System;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;

namespace UnitTests.Emit.C.Fakes
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
