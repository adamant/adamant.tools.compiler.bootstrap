using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ForeachExpressionSyntax : ExpressionSyntax
    {
        public bool MutableBinding { get; }
        [NotNull] public IIdentifierToken Identifier { get; }
        [CanBeNull] public ExpressionSyntax Type { get; }
        [NotNull] public ExpressionSyntax InExpression { get; }
        [NotNull] public BlockSyntax Block { get; }

        public ForeachExpressionSyntax(
            TextSpan span,
            bool mutableBinding,
            [NotNull] IIdentifierToken identifier,
            [CanBeNull] ExpressionSyntax type,
            [NotNull] ExpressionSyntax inExpression,
            [NotNull] BlockSyntax block)
            : base(span)
        {
            MutableBinding = mutableBinding;
            Identifier = identifier;
            InExpression = inExpression;
            Block = block;
            Type = type;
        }

        public override string ToString()
        {
            var binding = MutableBinding ? "var " : "";
            return $"foreach {binding}{Identifier}: {Type} in {InExpression} {Block}";
        }
    }
}
