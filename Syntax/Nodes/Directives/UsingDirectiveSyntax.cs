using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives
{
    public class UsingDirectiveSyntax : SyntaxBranchNode
    {
        public Token UsingKeyword { get; }
        public NameSyntax Name { get; }
        public Token Semicolon { get; }

        public UsingDirectiveSyntax(Token usingKeyword, NameSyntax name, Token semicolon)
            : base(usingKeyword, name, semicolon)
        {
            UsingKeyword = usingKeyword;
            Name = name;
            Semicolon = semicolon;
        }
    }
}
