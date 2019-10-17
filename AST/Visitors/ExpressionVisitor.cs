using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public abstract class ExpressionVisitor<A>
    {
        public void VisitStatement(IStatementSyntax? statement, A args)
        {
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    VisitVariableDeclarationStatement(variableDeclaration, args);
                    break;
                case IExpressionStatementSyntax expressionStatement:
                    VisitExpressionStatement(expressionStatement, args);
                    break;
                case IResultStatementSyntax resultStatement:
                    VisitResultStatement(resultStatement, args);
                    break;
                case null:
                    // Ignore
                    break;
            }
        }

        public virtual void VisitExpressionStatement(IExpressionStatementSyntax expressionStatement, A args)
        {
            VisitExpression(expressionStatement.Expression, args);
        }

        public virtual void VisitVariableDeclarationStatement(IVariableDeclarationStatementSyntax variableDeclaration, A args)
        {
            VisitType(variableDeclaration.TypeSyntax, args);
            VisitExpression(variableDeclaration.Initializer, args);
        }

        public virtual void VisitExpression(IExpressionSyntax? expression, A args)
        {
            switch (expression)
            {
                default:
                    throw ExhaustiveMatch.Failed(expression);
                case null:
                    // Ignore
                    break;
                case ILifetimeExpressionSyntax lifetimeExpression:
                    VisitLifetimeExpression(lifetimeExpression, args);
                    break;
                case IBinaryOperatorExpressionSyntax binaryOperatorExpression:
                    VisitBinaryOperatorExpression(binaryOperatorExpression, args);
                    break;
                case IReturnExpressionSyntax returnExpression:
                    VisitReturnExpression(returnExpression, args);
                    break;
                case IInvocationExpressionSyntax invocationExpression:
                    VisitInvocationExpression(invocationExpression, args);
                    break;
                case ILiteralExpressionSyntax literalExpression:
                    VisitLiteralExpression(literalExpression, args);
                    break;
                case INameExpressionSyntax nameExpression:
                    VisitNameExpression(nameExpression, args);
                    break;
                case IUnaryOperatorExpressionSyntax unaryOperatorExpression:
                    VisitUnaryOperatorExpression(unaryOperatorExpression, args);
                    break;
                case IAssignmentExpressionSyntax assignmentExpression:
                    VisitAssignmentExpression(assignmentExpression, args);
                    break;
                case IBlockExpressionSyntax block:
                    VisitBlockExpression(block, args);
                    break;
                case INewObjectExpressionSyntax newObjectExpression:
                    VisitNewObjectExpression(newObjectExpression, args);
                    break;
                case IMemberAccessExpressionSyntax memberAccessExpression:
                    VisitMemberAccessExpression(memberAccessExpression, args);
                    break;
                case IUnsafeExpressionSyntax unsafeExpression:
                    VisitUnsafeExpression(unsafeExpression, args);
                    break;
                case ISelfExpressionSyntax selfExpression:
                    VisitSelfExpression(selfExpression, args);
                    break;
                case ILoopExpressionSyntax loopExpression:
                    VisitLoopExpression(loopExpression, args);
                    break;
                case IIfExpressionSyntax ifExpression:
                    VisitIfExpression(ifExpression, args);
                    break;
                case IBreakExpressionSyntax breakExpression:
                    VisitBreakExpression(breakExpression, args);
                    break;
                case IImplicitConversionExpression implicitConversionExpression:
                    VisitImplicitConversionExpression(implicitConversionExpression, args);
                    break;
                case IForeachExpressionSyntax foreachExpression:
                    VisitForeachExpression(foreachExpression, args);
                    break;
                case INextExpressionSyntax nextExpression:
                    VisitNextExpression(nextExpression, args);
                    break;
                case IWhileExpressionSyntax whileExpression:
                    VisitWhileExpression(whileExpression, args);
                    break;
            }
        }

        public virtual void VisitLifetimeExpression(ILifetimeExpressionSyntax lifetimeExpression, A args)
        {
        }

        public void VisitType(ITypeSyntax? type, A args)
        {
            switch (type)
            {
                default:
                    throw ExhaustiveMatch.Failed(type);
                case null:
                    // Do nothing
                    break;
                case ITypeNameSyntax typeName:
                    VisitTypeName(typeName, args);
                    break;
                case IMutableTypeSyntax mutableType:
                    VisitMutableType(mutableType, args);
                    break;
                case IReferenceLifetimeTypeSyntax referenceLifetimeType:
                    VisitReferenceLifetimeType(referenceLifetimeType, args);
                    break;
            }
        }

        public virtual void VisitTypeName(ITypeNameSyntax typeName, A args)
        {
        }

        public virtual void VisitWhileExpression(IWhileExpressionSyntax whileExpression, A args)
        {
            VisitExpression(whileExpression.Condition, args);
            VisitExpression(whileExpression.Block, args);
        }

        public virtual void VisitNextExpression(INextExpressionSyntax nextExpression, A args)
        {
        }

        public virtual void VisitForeachExpression(IForeachExpressionSyntax foreachExpression, A args)
        {
            VisitType(foreachExpression.TypeSyntax, args);
            VisitExpression(foreachExpression.InExpression, args);
            VisitExpression(foreachExpression.Block, args);
        }

        public virtual void VisitMoveTransfer(IMoveTransferSyntax moveTransfer, A args)
        {
            VisitExpression(moveTransfer.Expression, args);
        }

        public virtual void VisitMutableType(IMutableTypeSyntax mutableType, A args)
        {
            VisitType(mutableType.Referent, args);
        }

        public void VisitImplicitConversionExpression(IImplicitConversionExpression? implicitConversionExpression, A args)
        {
            switch (implicitConversionExpression)
            {
                default:
                    throw ExhaustiveMatch.Failed(implicitConversionExpression);
                case null:
                    // Ignore
                    break;
                case IImplicitNumericConversionExpression implicitNumericConversionExpression:
                    VisitImplicitNumericConversionExpression(implicitNumericConversionExpression, args);
                    break;
                case IImplicitStringLiteralConversionExpression implicitLiteralConversionExpression:
                    VisitImplicitStringLiteralConversionExpression(implicitLiteralConversionExpression, args);
                    break;
                case IImplicitImmutabilityConversionExpression implicitImmutabilityConversionExpression:
                    VisitImplicitImmutabilityConversionExpression(implicitImmutabilityConversionExpression, args);
                    break;
                case IImplicitNoneConversionExpression implicitNoneConversionExpression:
                    VisitImplicitNoneConversionExpression(implicitNoneConversionExpression, args);
                    break;
                case IImplicitOptionalConversionExpression implicitOptionalConversionExpression:
                    VisitImplicitOptionalConversionExpression(implicitOptionalConversionExpression, args);
                    break;
            }
        }

        public virtual void VisitImplicitOptionalConversionExpression(IImplicitOptionalConversionExpression implicitOptionalConversionExpression, A args)
        {
            VisitExpression(implicitOptionalConversionExpression.Expression, args);
        }

        public virtual void VisitImplicitNoneConversionExpression(IImplicitNoneConversionExpression implicitNoneConversionExpression, A args)
        {
            VisitExpression(implicitNoneConversionExpression.Expression, args);
        }

        public virtual void VisitImplicitImmutabilityConversionExpression(IImplicitImmutabilityConversionExpression implicitImmutabilityConversionExpression, A args)
        {
            VisitExpression(implicitImmutabilityConversionExpression.Expression, args);
        }

        public virtual void VisitImplicitStringLiteralConversionExpression(IImplicitStringLiteralConversionExpression implicitStringLiteralConversionExpression, A args)
        {
            VisitExpression(implicitStringLiteralConversionExpression.Expression, args);
        }

        public virtual void VisitImplicitNumericConversionExpression(IImplicitNumericConversionExpression implicitNumericConversionExpression, A args)
        {
            VisitExpression(implicitNumericConversionExpression.Expression, args);
        }

        public virtual void VisitBreakExpression(IBreakExpressionSyntax breakExpression, A args)
        {
            VisitExpression(breakExpression.Value, args);
        }

        public virtual void VisitResultStatement(IResultStatementSyntax resultStatement, A args)
        {
            VisitExpression(resultStatement.Expression, args);
        }

        public virtual void VisitIfExpression(IIfExpressionSyntax ifExpression, A args)
        {
            VisitExpression(ifExpression.Condition, args);
            VisitBlockOrResult(ifExpression.ThenBlock, args);
            VisitElseClause(ifExpression.ElseClause, args);
        }

        public void VisitBlockOrResult(IBlockOrResultSyntax? blockOrResult, A args)
        {
            switch (blockOrResult)
            {
                default:
                    throw ExhaustiveMatch.Failed(blockOrResult);
                case null:
                    // ignore
                    break;
                case IBlockExpressionSyntax blockExpression:
                    VisitBlockExpression(blockExpression, args);
                    break;
                case IResultStatementSyntax resultStatement:
                    VisitResultStatement(resultStatement, args);
                    break;
            }
        }

        public void VisitElseClause(IElseClauseSyntax? elseClause, A args)
        {
            switch (elseClause)
            {
                default:
                    throw ExhaustiveMatch.Failed(elseClause);
                case null:
                    // ignore
                    break;
                case IBlockOrResultSyntax blockOrResult:
                    VisitBlockOrResult(blockOrResult, args);
                    break;
                case IIfExpressionSyntax ifExpression:
                    VisitIfExpression(ifExpression, args);
                    break;
            }
        }

        public virtual void VisitTransfer(ITransferSyntax? transfer, A args)
        {
            switch (transfer)
            {
                default:
                    throw ExhaustiveMatch.Failed(transfer);
                case null:
                    // ignore
                    break;
                case IImmutableTransferSyntax immutableTransfer:
                    VisitImmutableTransfer(immutableTransfer, args);
                    break;
                case IMutableTransferSyntax mutableTransfer:
                    VisitMutableTransfer(mutableTransfer, args);
                    break;
                case IMoveTransferSyntax moveTransfer:
                    VisitMoveTransfer(moveTransfer, args);
                    break;
            }
        }

        public virtual void VisitMutableTransfer(IMutableTransferSyntax mutableTransfer, A args)
        {
            VisitExpression(mutableTransfer.Expression, args);
        }

        public virtual void VisitImmutableTransfer(IImmutableTransferSyntax immutableTransfer, A args)
        {
            VisitExpression(immutableTransfer.Expression, args);
        }

        public virtual void VisitLoopExpression(ILoopExpressionSyntax loopExpression, A args)
        {
            VisitExpression(loopExpression.Block, args);
        }

        public virtual void VisitSelfExpression(ISelfExpressionSyntax selfExpression, A args)
        {
        }

        public virtual void VisitUnsafeExpression(IUnsafeExpressionSyntax unsafeExpression, A args)
        {
            VisitExpression(unsafeExpression.Expression, args);
        }

        public virtual void VisitMemberAccessExpression(IMemberAccessExpressionSyntax memberAccessExpression, A args)
        {
            VisitExpression(memberAccessExpression.Expression, args);
        }

        public virtual void VisitNewObjectExpression(INewObjectExpressionSyntax newObjectExpression, A args)
        {
            VisitType(newObjectExpression.TypeSyntax, args);
            foreach (var argument in newObjectExpression.Arguments)
                VisitTransfer(argument, args);
        }

        public virtual void VisitReferenceLifetimeType(IReferenceLifetimeTypeSyntax referenceLifetimeType, A args)
        {
            VisitType(referenceLifetimeType.ReferentType, args);
        }

        public virtual void VisitBlockExpression(IBlockExpressionSyntax block, A args)
        {
            foreach (var statement in block.Statements)
                VisitStatement(statement, args);
        }

        public virtual void VisitAssignmentExpression(IAssignmentExpressionSyntax assignmentExpression, A args)
        {
            VisitExpression(assignmentExpression.LeftOperand, args);
            VisitTransfer(assignmentExpression.RightOperand, args);
        }

        public virtual void VisitUnaryOperatorExpression(IUnaryOperatorExpressionSyntax unaryOperatorExpression, A args)
        {
            VisitExpression(unaryOperatorExpression.Operand, args);
        }

        public virtual void VisitNameExpression(INameExpressionSyntax nameExpression, A args)
        {
        }

        public virtual void VisitLiteralExpression(ILiteralExpressionSyntax literalExpression, A args)
        {
            // TODO this should dispatch on the type of literal
            //throw new NotImplementedException();
        }

        public void VisitInvocationExpression(IInvocationExpressionSyntax? invocation, A args)
        {
            switch (invocation)
            {
                default:
                    throw ExhaustiveMatch.Failed(invocation);
                case null:
                    // ignore
                    break;
                case IMethodInvocationExpressionSyntax methodInvocation:
                    VisitMethodInvocation(methodInvocation, args);
                    break;
                case IAssociatedFunctionInvocationExpressionSyntax associatedFunctionInvocation:
                    VisitAssociatedFunctionInvocation(associatedFunctionInvocation, args);
                    break;
                case IFunctionInvocationExpressionSyntax functionInvocation:
                    VisitFunctionInvocation(functionInvocation, args);
                    break;
            }
        }

        public virtual void VisitFunctionInvocation(IFunctionInvocationExpressionSyntax functionInvocationExpression, A args)
        {
            //VisitNameExpression(functionInvocationExpression.FunctionNameSyntax, args);
            foreach (var argument in functionInvocationExpression.Arguments)
                VisitTransfer(argument, args);
        }

        public virtual void VisitAssociatedFunctionInvocation(IAssociatedFunctionInvocationExpressionSyntax associatedFunctionInvocationExpression, A args)
        {
            foreach (var argument in associatedFunctionInvocationExpression.Arguments)
                VisitTransfer(argument, args);
        }

        public virtual void VisitMethodInvocation(IMethodInvocationExpressionSyntax methodInvocationExpression, A args)
        {
            VisitExpression(methodInvocationExpression.Target, args);
            foreach (var argument in methodInvocationExpression.Arguments)
                VisitTransfer(argument, args);
        }

        public virtual void VisitReturnExpression(IReturnExpressionSyntax returnExpression, A args)
        {
            VisitTransfer(returnExpression.ReturnValue, args);
        }

        public virtual void VisitBinaryOperatorExpression(IBinaryOperatorExpressionSyntax binaryOperatorExpression, A args)
        {
            VisitExpression(binaryOperatorExpression.LeftOperand, args);
            VisitExpression(binaryOperatorExpression.RightOperand, args);
        }
    }
}
