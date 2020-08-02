using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IFieldAccessExpressionSyntax
    {
        ref IExpressionSyntax ContextExpression { get; }
        [DisallowNull] IBindingMetadata? ReferencedBinding { get; set; }
    }
}
