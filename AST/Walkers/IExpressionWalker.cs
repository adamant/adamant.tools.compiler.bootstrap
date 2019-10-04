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

        void Enter(IIntegerLiteralExpressionSyntax integerLiteralExpression);
        void Exit(IIntegerLiteralExpressionSyntax integerLiteralExpression);

        void Enter(IMethodInvocationExpressionSyntax methodInvocationExpression);
        void Exit(IMethodInvocationExpressionSyntax methodInvocationExpression);

        void Enter(IAssignmentExpressionSyntax assignmentExpression);
        void Exit(IAssignmentExpressionSyntax assignmentExpression);

        void Enter(INewObjectExpressionSyntax newObjectExpression);
        void Exit(INewObjectExpressionSyntax newObjectExpression);

        void Enter(IBoolLiteralExpressionSyntax boolLiteralExpression);
        void Exit(IBoolLiteralExpressionSyntax boolLiteralExpression);

        void Enter(IIfExpressionSyntax ifExpression);
        void Exit(IIfExpressionSyntax ifExpression);

        void Enter(IBinaryOperatorExpressionSyntax binaryOperatorExpression);
        void Exit(IBinaryOperatorExpressionSyntax binaryOperatorExpression);

        void Enter(IUnaryOperatorExpressionSyntax unaryOperatorExpression);
        void Exit(IUnaryOperatorExpressionSyntax unaryOperatorExpression);

        void Enter(ILoopExpressionSyntax loopExpressionSyntax);
        void Exit(ILoopExpressionSyntax loopExpressionSyntax);

        void Enter(IWhileExpressionSyntax whileExpression);
        void Exit(IWhileExpressionSyntax whileExpression);

        void Enter(INoneLiteralExpressionSyntax noneLiteralExpression);
        void Exit(INoneLiteralExpressionSyntax noneLiteralExpression);

        void Enter(ISelfExpressionSyntax selfExpression);
        void Exit(ISelfExpressionSyntax selfExpression);

        void Enter(INextExpressionSyntax nextExpression);
        void Exit(INextExpressionSyntax nextExpression);

        void Enter(IMoveExpressionSyntax moveExpression);
        void Exit(IMoveExpressionSyntax moveExpression);

        void Enter(IMemberAccessExpressionSyntax memberAccessExpression);
        void Exit(IMemberAccessExpressionSyntax memberAccessExpression);

        void Enter(IBreakExpressionSyntax breakExpression);
        void Exit(IBreakExpressionSyntax breakExpression);

        void Enter(IForeachExpressionSyntax foreachExpression);
        void Exit(IForeachExpressionSyntax foreachExpression);
    }
}
