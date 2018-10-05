using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class CompilationUnitNamespaceSyntax : SyntaxNode
    {
        public SimpleToken NamespaceKeyword { get; }
        public NameSyntax Name { get; }
        public SimpleToken Semicolon { get; }

        public CompilationUnitNamespaceSyntax(SimpleToken namespaceKeyword, NameSyntax name, SimpleToken semicolon)
        {
            NamespaceKeyword = namespaceKeyword;
            Name = name;
            Semicolon = semicolon;
        }
    }
}
