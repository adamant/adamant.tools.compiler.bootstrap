using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IMethodInvocationExpressionSyntax),
        typeof(IFunctionInvocationExpressionSyntax),
        typeof(IAssociatedFunctionInvocationExpressionSyntax))]
    public interface IInvocationExpressionSyntax : IExpressionSyntax
    {
        Name FullName { get; }
        FixedList<IArgumentSyntax> Arguments { get; }
    }
}
