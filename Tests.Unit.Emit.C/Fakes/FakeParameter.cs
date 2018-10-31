using System;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Emit.C.Fakes
{
    public class FakeParameter : Parameter
    {
        private readonly Guid instance = Guid.NewGuid();

        public FakeParameter()
            : base(false, new QualifiedName("fake"), new FakeAdamantType())
        {
        }

        public override string ToString()
        {
            return $"⧼Fake Parameter {instance}⧽";
        }
    }
}
