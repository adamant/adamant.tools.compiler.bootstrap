namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public abstract class StatementWalker : IStatementWalker
    {
        public bool ShouldSkip(IStatementSyntax declaration)
        {
            return false;
        }

        public abstract void Enter(IVariableDeclarationStatementSyntax variableDeclaration);
        public virtual void Exit(IVariableDeclarationStatementSyntax variableDeclaration) { }

        public abstract void Enter(IExpressionStatementSyntax expressionStatement);
        public virtual void Exit(IExpressionStatementSyntax expressionStatement) { }

        public abstract void Enter(IResultStatementSyntax resultStatement);
        public virtual void Exit(IResultStatementSyntax resultStatement) { }
    }
}
