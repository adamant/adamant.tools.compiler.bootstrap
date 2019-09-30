namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface INamedFunctionDeclarationSyntax : IFunctionDeclarationSyntax
    {
        bool IsExternalFunction { get; set; }
        TypeSyntax? ReturnTypeSyntax { get; }
    }
}
