using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class ReturnExpressionSyntax : ExpressionSyntax, IReturnExpressionSyntax
    {
        [DisallowNull]
        public ITransferSyntax? ReturnValue { get; }

        public ReturnExpressionSyntax(
            TextSpan span,
            ITransferSyntax? returnValue)
            : base(span)
        {
            ReturnValue = returnValue;
        }

        public override string ToString()
        {
            if (ReturnValue != null)
                return $"return {ReturnValue}";
            return "return";
        }
    }
}
