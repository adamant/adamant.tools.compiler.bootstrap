using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    /// <summary>
    /// The universal type all reference types can be converted to. A top type
    /// for reference and function types.
    /// </summary>
    /// <remarks>
    /// `Any` is "declared" mutable so that it can hold mutable references to
    /// mutable types.
    /// </remarks>
    public sealed class AnyType : ReferenceType
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

        public override bool Equals(DataType? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is AnyType otherType
                   && ReferenceCapability == otherType.ReferenceCapability;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(SpecialNames.Any, ReferenceCapability);
        }

        protected internal override Self To_ReturnsSelf(ReferenceCapability referenceCapability)
        {
            return new AnyType(referenceCapability);
        }
    }
}
