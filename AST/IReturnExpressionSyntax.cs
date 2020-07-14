using System.Diagnostics.CodeAnalysis;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IReturnExpressionSyntax : IExpressionSyntax
    {
        [DisallowNull] ref IExpressionSyntax? ReturnValue { get; }
    }
}
