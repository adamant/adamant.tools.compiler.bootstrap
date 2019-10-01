using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IVariableDeclarationStatementSyntax : IStatementSyntax, IBindingSymbol
    {
        TextSpan NameSpan { get; }
        SimpleName Name { get; }
        ITypeSyntax TypeSyntax { get; }
        new DataType? Type { get; set; }
        IExpressionSyntax Initializer { get; }
        ref IExpressionSyntax InitializerRef { get; }
    }
}
