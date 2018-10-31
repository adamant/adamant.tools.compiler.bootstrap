using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens.Identifiers;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations.Types.Enums
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
