namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public interface IExpressionWalker
    {
        bool ShouldSkip(IExpressionSyntax expression);

        void Enter(IUnsafeExpressionSyntax unsafeExpression);
        void Exit(IUnsafeExpressionSyntax unsafeExpression);

        void Enter(IBlockExpressionSyntax blockExpression);
        void Exit(IBlockExpressionSyntax blockExpression);

        void Enter(IFunctionInvocationExpressionSyntax functionInvocationExpression);
        void Exit(IFunctionInvocationExpressionSyntax functionInvocationExpression);

        void Enter(INameExpressionSyntax nameExpression);
        void Exit(INameExpressionSyntax nameExpression);

        void Enter(IStringLiteralExpressionSyntax stringLiteralExpression);
        void Exit(IStringLiteralExpressionSyntax stringLiteralExpression);

        void Enter(IReturnExpressionSyntax returnExpression);
        void Exit(IReturnExpressionSyntax returnExpression);
    }
}
