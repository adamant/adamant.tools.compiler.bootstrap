using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal abstract class InvocationExpressionSyntax : ExpressionSyntax, IInvocationExpressionSyntax
    {
        public Name FullName { get; }
        public FixedList<ITransferSyntax> Arguments { get; }

        private protected InvocationExpressionSyntax(
            TextSpan span,
            Name functionName,
            FixedList<ITransferSyntax> arguments)
            : base(span)
        {
            Arguments = arguments;
            FullName = functionName;
        }
    }
}
