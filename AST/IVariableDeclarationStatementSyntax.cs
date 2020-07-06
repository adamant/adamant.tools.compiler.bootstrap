using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IVariableDeclarationStatementSyntax : IBodyStatementSyntax, IBindingSymbol
    {
        TextSpan NameSpan { get; }
        SimpleName Name { get; }
        ITypeSyntax? TypeSyntax { get; }
        [DisallowNull] new DataType? Type { get; set; }
        bool InferMutableType { get; }
        [DisallowNull] ref IExpressionSyntax? Initializer { get; }
        bool VariableIsLiveAfter { get; set; }
    }
}
