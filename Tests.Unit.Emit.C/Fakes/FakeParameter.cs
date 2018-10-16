using System;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Emit.C.Fakes
{
    public class FakeParameter : Parameter
    {
        private readonly Guid instance = Guid.NewGuid();

        public FakeParameter()
            : base(false, "fake")
        {
        }

        public override string ToString()
        {
            return $"⧼Fake Parameter {instance}⧽";
        }
    }
}
