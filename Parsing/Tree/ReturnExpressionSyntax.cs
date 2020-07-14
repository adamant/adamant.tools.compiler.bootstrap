using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.FST;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ReturnExpressionSyntax : ExpressionSyntax, IReturnExpressionSyntax
    {
        private IExpressionSyntax? returnValue;
        [DisallowNull]
        public ref IExpressionSyntax? ReturnValue
        {
            [DebuggerStepThrough]
            get => ref returnValue;
        }

        public ReturnExpressionSyntax(
            TextSpan span,
            IExpressionSyntax? returnValue)
            : base(span, ExpressionSemantics.Never)
        {
            this.returnValue = returnValue;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            if (ReturnValue != null)
                return $"return {ReturnValue}";
            return "return";
        }
    }
}
