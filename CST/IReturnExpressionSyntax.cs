using System.Diagnostics.CodeAnalysis;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IReturnExpressionSyntax : IExpressionSyntax
    {
        [DisallowNull] ref IExpressionSyntax? ReturnValue { get; }
    }
}
