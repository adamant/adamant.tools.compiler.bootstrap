using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class VariableDeclarationStatementSyntax : StatementSyntax
    {
        public bool MutableBinding { get; }
        [NotNull] public IIdentifierToken Name { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }

        public VariableDeclarationStatementSyntax(
            bool mutableBinding,
            [NotNull] IIdentifierToken name,
            [CanBeNull] ExpressionSyntax typeExpression,
            [CanBeNull] ExpressionSyntax initializer)
        {
            MutableBinding = mutableBinding;
            Name = name;
            TypeExpression = typeExpression;
            Initializer = initializer;
        }
    }
}
