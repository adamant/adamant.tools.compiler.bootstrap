using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface IForeachExpressionSyntax : IExpressionSyntax, IBindingSymbol
    {
        SimpleName VariableName { get; }
        bool VariableIsLiveAfterAssignment { get; set; }
        ITypeSyntax? TypeSyntax { get; }
        /// <summary>
        /// The type of the foreach expression overall, not of the variable
        /// </summary>
        [DisallowNull] new DataType? Type { get; set; }
        [DisallowNull] DataType? VariableType { get; set; }
        ref IExpressionSyntax InExpression { get; }
        IBlockExpressionSyntax Block { get; }
    }
}
