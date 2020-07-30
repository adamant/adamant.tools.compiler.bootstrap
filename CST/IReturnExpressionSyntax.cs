using System.Diagnostics.CodeAnalysis;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IReturnExpressionSyntax : IExpressionSyntax
    {
        [DisallowNull] ref IExpressionSyntax? ReturnValue { get; }
    }
}
