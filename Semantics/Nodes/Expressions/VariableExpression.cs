using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions
{
    public class VariableExpression : Expression
    {
        public new IdentifierNameSyntax Syntax { get; }
        public string Name => Syntax.Identifier.Text;

        public VariableExpression(IdentifierNameSyntax syntax)
        {
            Syntax = syntax;
        }

        protected override SyntaxNode GetSyntax()
        {
            return Syntax;
        }
    }
}
