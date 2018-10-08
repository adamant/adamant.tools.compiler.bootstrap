using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class ParameterSyntax : SyntaxNode
    {
        public SimpleToken? VarKeyword { get; }
        public IdentifierToken Name { get; }
        public SimpleToken Colon { get; }
        public ExpressionSyntax TypeExpression { get; }

        public ParameterSyntax(
            SimpleToken? varKeyword,
            IdentifierToken name,
            SimpleToken colon,
            ExpressionSyntax typeExpression)
        {
            VarKeyword = varKeyword;
            Name = name;
            Colon = colon;
            TypeExpression = typeExpression;
        }
    }
}
