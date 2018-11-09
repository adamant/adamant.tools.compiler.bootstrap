using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class EnumVariantsSyntax : NonTerminal
    {
        [NotNull] public SyntaxList<EnumVariantSyntax> Variants { get; }
        [CanBeNull] public ISemicolonTokenPlace Semicolon { get; }

        public EnumVariantsSyntax(
            [NotNull] SyntaxList<EnumVariantSyntax> variants,
            [CanBeNull] ISemicolonTokenPlace semicolon)
        {
            Requires.NotNull(nameof(variants), variants);
            Variants = variants;
            Semicolon = semicolon;
        }
    }
}
