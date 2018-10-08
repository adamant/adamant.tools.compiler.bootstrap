using Adamant.Tools.Compiler.Bootstrap.Syntax.Lexing;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Parsing
{
    public class UsingDirectiveParser : IParser<UsingDirectiveSyntax>
    {
        private readonly IParser<NameSyntax> qualifiedNameParser;

        public UsingDirectiveParser(IParser<NameSyntax> qualifiedNameParser)
        {
            this.qualifiedNameParser = qualifiedNameParser;
        }

        public UsingDirectiveSyntax Parse(ITokenStream tokens)
        {
            var usingKeyword = tokens.ExpectSimple(TokenKind.UsingKeyword);
            var name = qualifiedNameParser.Parse(tokens);
            var semicolon = tokens.ExpectSimple(TokenKind.Semicolon);
            return new UsingDirectiveSyntax(usingKeyword, name, semicolon);
        }
    }
}
