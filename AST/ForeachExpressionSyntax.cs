using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ForeachExpressionSyntax : ExpressionSyntax
    {
        public bool MutableBinding { get; }
        [NotNull] public SimpleName VariableName { get; }
        [CanBeNull] public ExpressionSyntax TypeExpression { get; }
        [NotNull] public ExpressionSyntax InExpression { get; }
        [NotNull] public BlockSyntax Block { get; }

        public ForeachExpressionSyntax(
            TextSpan span,
            bool mutableBinding,
            [NotNull] string variableName,
            [CanBeNull] ExpressionSyntax typeExpression,
            [NotNull] ExpressionSyntax inExpression,
            [NotNull] BlockSyntax block)
            : base(span)
        {
            MutableBinding = mutableBinding;
            VariableName = new SimpleName(variableName);
            InExpression = inExpression;
            Block = block;
            TypeExpression = typeExpression;
        }

        public override string ToString()
        {
            var binding = MutableBinding ? "var " : "";
            return $"foreach {binding}{VariableName}: {TypeExpression} in {InExpression} {Block}";
        }
    }
}
