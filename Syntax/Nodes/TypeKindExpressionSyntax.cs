using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class TypeKindExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ColonToken Colon { get; }
        [NotNull] public ITypeKindKeywordToken TypeKind { get; }

        public TypeKindExpressionSyntax([NotNull] ColonToken colon, [NotNull] ITypeKindKeywordToken typeKind)
            : base(TextSpan.Covering(colon.Span, typeKind.Span))
        {
            Requires.NotNull(nameof(colon), colon);
            Requires.NotNull(nameof(typeKind), typeKind);
            Colon = colon;
            TypeKind = typeKind;
        }
    }
}
