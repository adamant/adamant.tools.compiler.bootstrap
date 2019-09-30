using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(MethodInvocationSyntax),
        typeof(FunctionInvocationSyntax),
        typeof(AssociatedFunctionInvocationSyntax))]
    public abstract class InvocationSyntax : ExpressionSyntax
    {
        public Name FunctionName { get; }
        public FixedList<IArgumentSyntax> Arguments { get; }

        private protected InvocationSyntax(
            TextSpan span,
            Name functionName,
            FixedList<IArgumentSyntax> arguments)
            : base(span)
        {
            Arguments = arguments;
            FunctionName = functionName;
        }
    }
}
