using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class VariableDeclarationStatementSyntax : StatementSyntax
    {

        public SimpleToken Binding { get; }
        public IdentifierToken Name { get; }
        public SimpleToken Colon { get; }
        public ExpressionSyntax TypeExpression { get; }
        public bool HasInitializer => Initializer != null;
        public new SimpleToken? Equals { get; }
        public ExpressionSyntax Initializer { get; }
        public SimpleToken Semicolon { get; }

        public VariableDeclarationStatementSyntax(
            SimpleToken binding,
            IdentifierToken name,
            SimpleToken colon,
            ExpressionSyntax typeExpression,
            SimpleToken? equals,
            ExpressionSyntax initializer,
            SimpleToken semicolon)
        {
            Binding = binding;
            Name = name;
            Colon = colon;
            TypeExpression = typeExpression;
            Equals = @equals;
            Initializer = initializer;
            Semicolon = semicolon;
        }
    }
}
