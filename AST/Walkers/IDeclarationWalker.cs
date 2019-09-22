namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public interface IDeclarationWalker
    {
        bool Enter(ClassDeclarationSyntax classDeclaration);
        void Exit(ClassDeclarationSyntax classDeclaration);
    }
}
