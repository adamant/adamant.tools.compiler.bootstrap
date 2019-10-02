namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public interface IDeclarationWalker
    {
        bool ShouldSkip(IDeclarationSyntax declaration);

        void Enter(IClassDeclarationSyntax classDeclaration);
        void Exit(IClassDeclarationSyntax classDeclaration);
    }
}
