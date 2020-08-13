using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class FieldAccessExpressionSyntax : ExpressionSyntax, IFieldAccessExpressionSyntax
    {
        private IExpressionSyntax contextExpression;
        public ref IExpressionSyntax ContextExpression => ref contextExpression;

        public AccessOperator AccessOperator { get; }
        public INameExpressionSyntax Field { get; }
        public IPromise<FieldSymbol?> ReferencedSymbol => Field.ReferencedSymbol.Select(s => (FieldSymbol?)s);

        public FieldAccessExpressionSyntax(
            TextSpan span,
            IExpressionSyntax contextExpression,
            AccessOperator accessOperator,
            INameExpressionSyntax field)
            : base(span)
        {
            this.contextExpression = contextExpression;
            AccessOperator = accessOperator;
            Field = field;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"{ContextExpression.ToGroupedString(ExpressionPrecedence)}.{Field}";
        }
    }
}
