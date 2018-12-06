using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ForeachExpressionSyntax : ExpressionSyntax
    {
        public bool MutableBinding { get; }
        public SimpleName VariableName { get; }
        public ExpressionSyntax TypeExpression { get; }
        public ExpressionSyntax InExpression { get; }
        public BlockSyntax Block { get; }

        public ForeachExpressionSyntax(
            TextSpan span,
            bool mutableBinding,
            string variableName,
            ExpressionSyntax typeExpression,
            ExpressionSyntax inExpression,
            BlockSyntax block)
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
