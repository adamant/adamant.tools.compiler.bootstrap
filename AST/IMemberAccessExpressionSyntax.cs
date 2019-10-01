using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IMemberAccessExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax? Expression { get; }
        AccessOperator AccessOperator { get; }
        INameSyntax Member { get; }
        [DisallowNull] ISymbol? ReferencedSymbol { get; set; }
    }
}
