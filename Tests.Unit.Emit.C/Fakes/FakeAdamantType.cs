using System;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Emit.C.Fakes
{
    public class FakeAdamantType : KnownType
    {
        private readonly Guid instance;

        public FakeAdamantType()
        {
            instance = Guid.NewGuid();
        }

        public override string ToString()
        {
            return $"⧼Fake Type {instance}⧽";
        }
    }
}
