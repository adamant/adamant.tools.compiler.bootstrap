using System.Diagnostics.CodeAnalysis;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IReturnExpressionSyntax : IExpressionSyntax
    {
        [DisallowNull] ITransferSyntax? ReturnValue { get; }
    }
}
