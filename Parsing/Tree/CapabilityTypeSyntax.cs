using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class CapabilityTypeSyntax : TypeSyntax, ICapabilityTypeSyntax
    {
        public ITypeSyntax ReferentType { get; }
        public ReferenceCapability Capability { get; }

        public CapabilityTypeSyntax(
            ReferenceCapability referenceCapability,
            ITypeSyntax referentType,
            TextSpan span)
            : base(span)
        {
            ReferentType = referentType;
            Capability = referenceCapability;
        }

        public override string ToString()
        {
            var capability = Capability.ToSourceString();
            if (capability.Length == 0) return ReferentType.ToString();

            return $"{capability} {ReferentType}";
        }
    }
}
