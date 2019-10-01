using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IMethodInvocationSyntax),
        typeof(IFunctionInvocationSyntax),
        typeof(IAssociatedFunctionInvocationSyntax))]
    public interface IInvocationSyntax : IExpressionSyntax
    {
        Name FunctionName { get; }
        FixedList<IArgumentSyntax> Arguments { get; }
    }
}
