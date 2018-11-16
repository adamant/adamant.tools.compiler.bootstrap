using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class EnumVariantSyntax : NonTerminal
    {
        [NotNull] public IIdentifierToken Identifier { get; }

        public EnumVariantSyntax([NotNull] IIdentifierToken identifier)
        {
            Identifier = identifier;
        }
    }
}
