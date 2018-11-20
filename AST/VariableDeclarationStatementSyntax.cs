using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class VariableDeclarationStatementSyntax : StatementSyntax
    {
        public bool MutableBinding { get; }
        [NotNull] public SimpleName Name { get; }
        public TextSpan NameSpan { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [CanBeNull] public ExpressionSyntax Initializer { get; }

        public VariableDeclarationStatementSyntax(
            bool mutableBinding,
            [NotNull] string name,
            TextSpan nameSpan,
            [CanBeNull] ExpressionSyntax typeExpression,
            [CanBeNull] ExpressionSyntax initializer)
        {
            MutableBinding = mutableBinding;
            Name = new SimpleName(name);
            NameSpan = nameSpan;
            TypeExpression = typeExpression;
            Initializer = initializer;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
