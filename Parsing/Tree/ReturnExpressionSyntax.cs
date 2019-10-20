using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ReturnExpressionSyntax : ExpressionSyntax, IReturnExpressionSyntax
    {
        private ITransferSyntax? returnValue;
        [DisallowNull] public ref ITransferSyntax? ReturnValue => ref returnValue;

        public ReturnExpressionSyntax(
            TextSpan span,
            ITransferSyntax? returnValue)
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
