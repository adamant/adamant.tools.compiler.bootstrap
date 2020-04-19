using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class AnyType : ReferenceType
    {
        private AnyType(Mutability mutability, ReferenceCapability referenceCapability)
            : base(true, mutability, referenceCapability)
        {
        }

        public override bool IsKnown => true;

        protected internal override Self AsImmutableReturnsSelf()
        {
            throw new System.NotImplementedException();
        }

        protected internal override Self WithCapabilityReturnsSelf(ReferenceCapability referenceCapability)
        {
            return new AnyType(Mutability, referenceCapability);
        }

        public override string ToString() => Mutability == Mutability.Mutable ? "mut Any" : "Any";
    }
}
