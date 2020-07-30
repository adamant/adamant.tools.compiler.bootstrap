using System.Diagnostics.CodeAnalysis;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IBreakExpressionSyntax : IExpressionSyntax
    {
        [DisallowNull] ref IExpressionSyntax? Value { get; }
    }
}
