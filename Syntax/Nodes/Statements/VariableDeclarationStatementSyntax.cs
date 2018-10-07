using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class VariableDeclarationStatementSyntax : StatementSyntax
    {

        public SimpleToken Binding { get; }
        public IdentifierToken Name { get; }
        public SimpleToken Colon { get; }
        public TypeSyntax Type { get; }
        public bool HasInitializer => Initializer != null;
        public new SimpleToken? Equals { get; }
        public ExpressionSyntax Initializer { get; }
        public SimpleToken Semicolon { get; }

        public VariableDeclarationStatementSyntax(
            SimpleToken binding,
            IdentifierToken name,
            SimpleToken colon,
            TypeSyntax type,
            SimpleToken? equals,
            ExpressionSyntax initializer,
            SimpleToken semicolon)
        {
            Binding = binding;
            Name = name;
            Colon = colon;
            Type = type;
            Equals = @equals;
            Initializer = initializer;
            Semicolon = semicolon;
        }
    }
}
