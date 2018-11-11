using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class EnumVariantsSyntax : NonTerminal
    {
        [NotNull] public FixedList<EnumVariantSyntax> Variants { get; }
        [CanBeNull] public ISemicolonTokenPlace Semicolon { get; }

        public EnumVariantsSyntax(
            [NotNull] FixedList<EnumVariantSyntax> variants,
            [CanBeNull] ISemicolonTokenPlace semicolon)
        {
            Variants = variants;
            Semicolon = semicolon;
        }
    }
}
