using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class CompilationUnitNamespaceSyntax : SyntaxBranchNode
    {
        public Token NamespaceKeyword { get; }
        public NameSyntax Name { get; }
        public Token Semicolon { get; }

        public CompilationUnitNamespaceSyntax(Token namespaceKeyword, NameSyntax name, Token semicolon)
            : base(namespaceKeyword, name, semicolon)
        {
            NamespaceKeyword = namespaceKeyword;
            Name = name;
            Semicolon = semicolon;
        }
    }
}
