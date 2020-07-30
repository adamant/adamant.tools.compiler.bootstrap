using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface IFieldAccessExpressionSyntax : IAssignableExpressionSyntax
    {
        ref IExpressionSyntax ContextExpression { get; }
        AccessOperator AccessOperator { get; }
        INameExpressionSyntax Field { get; }
        [DisallowNull] IBindingSymbol? ReferencedSymbol { get; set; }
    }
}
