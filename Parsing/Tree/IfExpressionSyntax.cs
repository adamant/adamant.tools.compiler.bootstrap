using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class IfExpressionSyntax : ExpressionSyntax, IIfExpressionSyntax
    {
        private IExpressionSyntax condition;

        public ref IExpressionSyntax Condition => ref condition;

        public IBlockOrResultSyntax ThenBlock { get; }
        public IElseClauseSyntax? ElseClause { get; }

        public IfExpressionSyntax(
            TextSpan span,
            IExpressionSyntax condition,
            IBlockOrResultSyntax thenBlock,
            IElseClauseSyntax? elseClause)
            : base(span)
        {
            this.condition = condition;
            ThenBlock = thenBlock;
            ElseClause = elseClause;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            if (ElseClause != null)
                return $"if {Condition} {ThenBlock} else {ElseClause}";
            return $"if {Condition} {ThenBlock}";
        }
    }
}
