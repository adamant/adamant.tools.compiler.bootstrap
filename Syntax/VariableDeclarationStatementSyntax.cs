using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class VariableDeclarationStatementSyntax : StatementSyntax
    {
        [NotNull] public IBindingToken Binding { get; }
        [NotNull] public IIdentifierTokenPlace Name { get; }
        [CanBeNull] public IColonTokenPlace Colon { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public new IEqualsToken Equals { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }
        [NotNull] public ISemicolonTokenPlace Semicolon { get; }

        public VariableDeclarationStatementSyntax(
            [NotNull] IBindingToken binding,
            [NotNull] IIdentifierTokenPlace name,
            [CanBeNull] IColonTokenPlace colon,
            [CanBeNull] ExpressionSyntax typeExpression,
            [CanBeNull] IEqualsToken equals,
            [CanBeNull] ExpressionSyntax initializer,
            [NotNull] ISemicolonTokenPlace semicolon)
        {
            Requires.NotNull(nameof(binding), binding);
            Requires.NotNull(nameof(name), name);
            Requires.NotNull(nameof(semicolon), semicolon);
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
