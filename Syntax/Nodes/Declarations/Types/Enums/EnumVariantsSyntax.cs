using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types.Enums
{
    public class EnumVariantsSyntax : SyntaxNode
    {
        [NotNull] public SyntaxList<EnumVariantSyntax> Variants { get; }
        [CanBeNull] public ISemicolonToken Semicolon { get; }

        public EnumVariantsSyntax(
            [NotNull] SyntaxList<EnumVariantSyntax> variants,
            [CanBeNull] ISemicolonToken semicolon)
        {
            Requires.NotNull(nameof(variants), variants);
            Variants = variants;
            Semicolon = semicolon;
        }
    }
}
