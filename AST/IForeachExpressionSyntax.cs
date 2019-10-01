using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IForeachExpressionSyntax : IExpressionSyntax, IBindingSymbol
    {
        SimpleName VariableName { get; }
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
