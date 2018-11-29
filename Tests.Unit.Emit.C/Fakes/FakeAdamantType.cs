using System;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Emit.C.Fakes
{
    public class FakeAdamantType : DataType
    {
        private readonly Guid instance;

        public FakeAdamantType()
        {
            instance = Guid.NewGuid();
        }

        public override bool IsResolved => true;

        public override string ToString()
        {
            return $"⧼Fake Type {instance}⧽";
        }
    }
}
