using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    [Closed(
        typeof(IMethodInvocationExpressionSyntax),
        typeof(IFunctionInvocationExpressionSyntax))]
    public interface IInvocationExpressionSyntax : IExpressionSyntax
    {
        Name FullName { get; }
        FixedList<IArgumentSyntax> Arguments { get; }
    }
}
