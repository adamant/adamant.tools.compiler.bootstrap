using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ReferenceCapabilityTypeSyntax : TypeSyntax, IReferenceCapabilityTypeSyntax
    {
        public ITypeSyntax ReferentType { get; }
        public ReferenceCapability ReferenceCapability { get; }

        public ReferenceCapabilityTypeSyntax(
            ReferenceCapability referenceCapability,
            ITypeSyntax referentType,
            TextSpan span)
            : base(span)
        {
            ReferentType = referentType;
            ReferenceCapability = referenceCapability;
        }

        public override string ToString()
        {
            var capability = ReferenceCapability.ToSourceString();
            if (capability.Length == 0) return ReferentType.ToString();

            return $"{capability} {ReferentType}";
        }
    }
}
