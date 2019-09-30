using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IFieldDeclarationSyntax : IMemberDeclarationSyntax, IBindingSymbol
    {
        TypeSyntax? TypeSyntax { get; }
        ExpressionSyntax? Initializer { get; }
    }
}
