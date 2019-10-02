namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public interface IStatementWalker
    {
        bool ShouldSkip(IStatementSyntax declaration);

        void Enter(IVariableDeclarationStatementSyntax variableDeclaration);
        void Exit(IVariableDeclarationStatementSyntax variableDeclaration);

        void Enter(IExpressionStatementSyntax expressionStatement);
        void Exit(IExpressionStatementSyntax expressionStatement);

        void Enter(IResultStatementSyntax resultStatement);
        void Exit(IResultStatementSyntax resultStatement);
    }
}
