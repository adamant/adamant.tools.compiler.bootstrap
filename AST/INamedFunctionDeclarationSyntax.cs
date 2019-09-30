namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface INamedFunctionDeclarationSyntax : IFunctionDeclarationSyntax, ICallableDeclarationSyntax
    {
        bool IsExternalFunction { get; set; }
        TypeSyntax? ReturnTypeSyntax { get; }
    }
}
