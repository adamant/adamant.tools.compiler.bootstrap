namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public interface IDeclarationWalker
    {
        bool Enter(IClassDeclarationSyntax classDeclaration);
        void Exit(IClassDeclarationSyntax classDeclaration);
    }
}
