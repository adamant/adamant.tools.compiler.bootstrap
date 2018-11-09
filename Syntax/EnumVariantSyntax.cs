using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class EnumVariantSyntax : NonTerminal
    {
        [NotNull] public IIdentifierTokenPlace Identifier { get; }
        [CanBeNull] public ICommaToken Comma { get; }

        public EnumVariantSyntax(
            [NotNull] IIdentifierTokenPlace identifier,
            [CanBeNull] ICommaToken comma)
        {
            Requires.NotNull(nameof(identifier), identifier);
            Identifier = identifier;
            Comma = comma;
        }
    }
}
