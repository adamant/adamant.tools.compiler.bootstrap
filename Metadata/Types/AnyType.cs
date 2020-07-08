using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// The universal type all reference types can be converted to.
    /// </summary>
    /// <remarks>
    /// `Any` is "declared" mutable so that it can hold mutable references to
    /// mutable types.
    /// </remarks>
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

        protected internal override Self To_ReturnsSelf(ReferenceCapability referenceCapability)
        {
            return new AnyType(referenceCapability);
        }
    }
}
