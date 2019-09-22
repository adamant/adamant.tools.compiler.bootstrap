using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ReferenceLifetimeSyntax : TypeSyntax
    {
        public TypeSyntax ReferentType { get; }
        public SimpleName Lifetime { get; }

        public ReferenceLifetimeSyntax(
            TypeSyntax referentType,
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
