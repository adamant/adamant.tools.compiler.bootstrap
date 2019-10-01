using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IForeachExpressionSyntax : IExpressionSyntax, IBindingSymbol
    {
        ITypeSyntax? TypeSyntax { get; }
        new DataType? Type { get; set; }
        [DisallowNull] DataType? VariableType { get; set; }
        ref IExpressionSyntax InExpression { get; }
        BlockSyntax Block { get; }
    }
}
