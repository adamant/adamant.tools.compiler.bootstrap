using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Directives
{
    public class UsingDirectiveSyntax : SyntaxNode
    {
        public SimpleToken UsingKeyword { get; }
        public NameSyntax Name { get; }
        public SimpleToken Semicolon { get; }

        public UsingDirectiveSyntax(SimpleToken usingKeyword, NameSyntax name, SimpleToken semicolon)
        {
            UsingKeyword = usingKeyword;
            Name = name;
            Semicolon = semicolon;
        }
    }
}
