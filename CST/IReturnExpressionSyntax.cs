using System.Diagnostics.CodeAnalysis;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IReturnExpressionSyntax
    {
        [DisallowNull] ref IExpressionSyntax? ReturnValue { get; }
    }
}
