using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ReferenceLifetimeSyntax : TypeSyntax, IReferenceLifetimeSyntax
    {
        public ITypeSyntax ReferentType { get; }
        public SimpleName Lifetime { get; }

        public ReferenceLifetimeSyntax(
            ITypeSyntax referentType,
            TextSpan nameSpan,
            SimpleName lifetime)
            : base(TextSpan.Covering(referentType.Span, nameSpan))
        {
            ReferentType = referentType;
            Lifetime = lifetime;
        }

        public override string ToString()
        {
            return $"{ReferentType}${Lifetime}";
        }
    }
}
