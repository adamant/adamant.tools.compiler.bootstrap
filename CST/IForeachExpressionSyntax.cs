using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface IForeachExpressionSyntax : IBindingMetadata
    {
        bool VariableIsLiveAfterAssignment { get; set; }
        /// <summary>
        /// The type of the foreach expression overall, not of the variable
        /// </summary>
        [DisallowNull] new DataType? Type { get; set; }
        [DisallowNull] DataType? VariableType { get; set; }
        ref IExpressionSyntax InExpression { get; }
    }
}
