using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ReturnExpressionSyntax : ExpressionSyntax, IReturnExpressionSyntax
    {
        private IExpressionSyntax? returnValue;
        [DisallowNull] public ref IExpressionSyntax? ReturnValue => ref returnValue;

        public ReturnExpressionSyntax(
            TextSpan span,
            IExpressionSyntax? returnValue)
            : base(span)
        {
            this.returnValue = returnValue;
        }

        public override string ToString()
        {
            if (ReturnValue != null)
                return $"return {ReturnValue}";
            return "return";
        }
    }
}
