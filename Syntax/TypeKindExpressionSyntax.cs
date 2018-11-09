using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class TypeKindExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IColonToken Colon { get; }
        [NotNull] public ITypeKindKeywordTokenPlace TypeKind { get; }

        public TypeKindExpressionSyntax([NotNull] IColonToken colon, [NotNull] ITypeKindKeywordTokenPlace typeKind)
            : base(TextSpan.Covering(colon.Span, typeKind.Span))
        {
            Requires.NotNull(nameof(colon), colon);
            Requires.NotNull(nameof(typeKind), typeKind);
            Colon = colon;
            TypeKind = typeKind;
        }
    }
}
