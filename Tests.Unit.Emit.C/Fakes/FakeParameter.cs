using System;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Emit.C.Fakes
{
    public class FakeParameter : Parameter
    {
        private readonly Guid instance;

        public FakeParameter()
        {
            instance = Guid.NewGuid();
        }

        public override string ToString()
        {
            return $"⧼Fake Parameter {instance}⧽";
        }
    }
}
