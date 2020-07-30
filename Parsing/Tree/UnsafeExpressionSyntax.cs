using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class UnsafeExpressionSyntax : ExpressionSyntax, IUnsafeExpressionSyntax
    {
        private IExpressionSyntax expression;

        public ref IExpressionSyntax Expression => ref expression;

        public UnsafeExpressionSyntax(TextSpan span, IExpressionSyntax expression)
            : base(span)
        {
            this.expression = expression;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"unsafe ({Expression})";
        }
    }
}
