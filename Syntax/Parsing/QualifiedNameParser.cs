using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class QualifiedNameParser : IParser<NameSyntax>
    {
        [MustUseReturnValue]
        public NameSyntax Parse(ITokenStream tokens)
        {
            NameSyntax qualifiedName = new IdentifierNameSyntax(tokens.ExpectIdentifier());
            while (tokens.CurrentIs(TokenKind.Dot))
            {
                var dot = tokens.ExpectSimple(TokenKind.Dot);
                var name = new IdentifierNameSyntax(tokens.ExpectIdentifier());
                qualifiedName = new QualifiedNameSyntax(qualifiedName, dot, name);
            }
            return qualifiedName;
        }
    }
}
