using System.Diagnostics.CodeAnalysis;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IBreakExpressionSyntax : IExpressionSyntax
    {
        [DisallowNull] ref IExpressionSyntax? Value { get; }
    }
}
