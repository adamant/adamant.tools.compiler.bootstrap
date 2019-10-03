using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    [SuppressMessage("Performance", "CA1812:Class Never Instantiated")]
    internal class ReferenceLifetimeTypeSyntax : TypeSyntax, IReferenceLifetimeTypeSyntax
    {
        public ITypeSyntax ReferentType { get; }
        public SimpleName Lifetime { get; }

        public ReferenceLifetimeTypeSyntax(
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
