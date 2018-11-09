using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class EnumVariantSyntax : SyntaxNode
    {
        [NotNull] public IIdentifierToken Identifier { get; }
        [CanBeNull] public CommaToken Comma { get; }

        public EnumVariantSyntax(
            [NotNull] IIdentifierToken identifier,
            [CanBeNull] CommaToken comma)
        {
            Requires.NotNull(nameof(identifier), identifier);
            Identifier = identifier;
            Comma = comma;
        }
    }
}
