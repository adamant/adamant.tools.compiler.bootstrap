using System.Diagnostics.CodeAnalysis;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IBreakExpressionSyntax : IExpressionSyntax
    {
        [DisallowNull] ref IExpressionSyntax? Value { get; }
    }
}
