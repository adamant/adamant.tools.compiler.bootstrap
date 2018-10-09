using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class VariableDeclarationStatementSyntax : StatementSyntax
    {
        [CanBeNull]
        public KeywordToken Binding { get; }

        [CanBeNull]
        public IdentifierToken Name { get; }

        [CanBeNull]
        public ColonToken Colon { get; }

        public ExpressionSyntax TypeExpression { get; }

        public bool HasInitializer => Initializer != null;

        [CanBeNull]
        public new EqualsToken Equals { get; }

        [CanBeNull]
        public ExpressionSyntax Initializer { get; }

        [CanBeNull]
        public SemicolonToken Semicolon { get; }

        public VariableDeclarationStatementSyntax(
            [CanBeNull] KeywordToken binding,
            [CanBeNull] IdentifierToken name,
            [CanBeNull] ColonToken colon,
            ExpressionSyntax typeExpression,
            [CanBeNull] EqualsToken equals,
            [CanBeNull] ExpressionSyntax initializer,
            [CanBeNull] SemicolonToken semicolon)
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
