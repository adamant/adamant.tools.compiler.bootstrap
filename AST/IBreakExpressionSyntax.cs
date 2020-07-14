using System.Diagnostics.CodeAnalysis;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IBreakExpressionSyntax : IExpressionSyntax
    {
        [DisallowNull] ref IExpressionSyntax? Value { get; }
    }
}
