using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ReturnExpressionSyntax : ExpressionSyntax, IReturnExpressionSyntax
    {
        private IExpressionSyntax? value;
        [DisallowNull]
        public ref IExpressionSyntax? Value
        {
            [DebuggerStepThrough]
            get => ref value;
        }

        public ReturnExpressionSyntax(
            TextSpan span,
            IExpressionSyntax? value)
            : base(span, ExpressionSemantics.Never)
        {
            this.value = value;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            return Value is null ? "return" : $"return {Value}";
        }
    }
}
