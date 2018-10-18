using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements
{
    public class VariableDeclarationStatementSyntax : StatementSyntax
    {
        [NotNull] public IBindingKeywordToken Binding { get; }
        [NotNull] public IIdentifierToken Name { get; }
        [CanBeNull] public IColonToken Colon { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        public bool HasInitializer => Initializer != null;
        [CanBeNull] public new EqualsToken Equals { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }
        [NotNull] public ISemicolonToken Semicolon { get; }

        public VariableDeclarationStatementSyntax(
            [NotNull] IBindingKeywordToken binding,
            [NotNull] IIdentifierToken name,
            [CanBeNull] IColonToken colon,
            [CanBeNull] ExpressionSyntax typeExpression,
            [CanBeNull] EqualsToken equals,
            [CanBeNull] ExpressionSyntax initializer,
            [NotNull] ISemicolonToken semicolon)
        {
            Requires.NotNull(nameof(binding), binding);
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
