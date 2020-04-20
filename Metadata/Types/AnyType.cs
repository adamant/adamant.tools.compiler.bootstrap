using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    public class AnyType : ReferenceType
    {
        public AnyType(ReferenceCapability referenceCapability)
            : base(true, referenceCapability)
        {
        }

        public override bool IsKnown => true;

        public override string ToString()
        {
            var capability = ReferenceCapability.ToSourceString();
            if (capability.Length == 0) return "Any";
            return $"{capability} Any";
        }

        protected internal override Self WithCapabilityReturnsSelf(ReferenceCapability referenceCapability)
        {
            return new AnyType(referenceCapability);
        }
    }
}
