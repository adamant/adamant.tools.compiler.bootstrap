using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IFieldAccessExpressionSyntax : IExpressionSyntax
    {
        /// <summary>
        /// This expression is null for implicit member access i.e. self and enums
        /// </summary>
        [DisallowNull]
        ref IExpressionSyntax? Expression { get; }
        AccessOperator AccessOperator { get; }
        INameExpressionSyntax Field { get; }
        [DisallowNull] IBindingSymbol? ReferencedSymbol { get; set; }
    }
}
