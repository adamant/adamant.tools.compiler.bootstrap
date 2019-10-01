using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public class ExpressionVisitor<A>
    {
        public virtual void VisitStatement(IStatementSyntax statement, A args)
        {
            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    VisitVariableDeclarationStatement(variableDeclaration, args);
                    break;
                case IExpressionStatementSyntax expressionStatement:
                    VisitExpression(expressionStatement.Expression, args);
                    break;
                case IResultStatementSyntax resultStatement:
                    VisitResultStatement(resultStatement, args);
                    break;
                case null:
                    // Ignore
                    break;
            }
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
                    throw NonExhaustiveMatchException.For(expression);
                case IBinaryExpressionSyntax binaryExpression:
                    VisitBinaryExpression(binaryExpression, args);
                    break;
                case IReturnExpressionSyntax returnExpression:
                    VisitReturnExpression(returnExpression, args);
                    break;
                case InvocationSyntax invocation:
                    VisitInvocation(invocation, args);
                    break;
                case ILiteralExpressionSyntax literalExpression:
                    VisitLiteralExpression(literalExpression, args);
                    break;
                case INameSyntax identifierName:
                    VisitName(identifierName, args);
                    break;
                case IUnaryExpressionSyntax unaryExpression:
                    VisitUnaryExpression(unaryExpression, args);
                    break;
                case IAssignmentExpressionSyntax assignmentExpression:
                    VisitAssignmentExpression(assignmentExpression, args);
                    break;
                case IBlockSyntax block:
                    VisitBlock(block, args);
                    break;
                case NewObjectExpressionSyntax newObjectExpression:
                    VisitNewObjectExpression(newObjectExpression, args);
                    break;
                case MemberAccessExpressionSyntax memberAccessExpression:
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
                case ImplicitConversionExpression implicitConversion:
                    VisitImplicitConversionExpression(implicitConversion, args);
                    break;
                case IMoveExpressionSyntax moveExpression:
                    VisitMoveExpression(moveExpression, args);
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
                case null:
                    // Ignore
                    break;
            }
        }

        public virtual void VisitType(ITypeSyntax? type, A args)
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
                case IReferenceLifetimeSyntax referenceLifetime:
                    VisitReferenceLifetime(referenceLifetime, args);
                    break;
                case ISelfTypeSyntax selfType:
                    VisitSelfType(selfType, args);
                    break;
            }
        }

        public virtual void VisitSelfType(ISelfTypeSyntax selfType, A args)
        {

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

        public virtual void VisitMoveExpression(IMoveExpressionSyntax moveExpression, A args)
        {
            VisitExpression(moveExpression.Expression, args);
        }

        public virtual void VisitMutableType(IMutableTypeSyntax mutableType, A args)
        {
            VisitType(mutableType.Referent, args);
        }

        public virtual void VisitImplicitConversionExpression(ImplicitConversionExpression implicitConversionExpression, A args)
        {
            switch (implicitConversionExpression)
            {
                case ImplicitNumericConversionExpression implicitNumericConversionExpression:
                    VisitImplicitNumericConversionExpression(implicitNumericConversionExpression, args);
                    break;
                case ImplicitStringLiteralConversionExpression implicitLiteralConversionExpression:
                    VisitImplicitStringLiteralConversionExpression(implicitLiteralConversionExpression, args);
                    break;
                case ImplicitImmutabilityConversionExpression implicitImmutabilityConversionExpression:
                    VisitImplicitImmutabilityConversionExpression(implicitImmutabilityConversionExpression, args);
                    break;
                case ImplicitNoneConversionExpression implicitNoneConversionExpression:
                    VisitImplicitNoneConversionExpression(implicitNoneConversionExpression, args);
                    break;
                case ImplicitOptionalConversionExpression implicitOptionalConversionExpression:
                    VisitImplicitOptionalConversionExpression(implicitOptionalConversionExpression, args);
                    break;
                case null:
                    // Ignore
                    break;
                default:
                    throw NonExhaustiveMatchException.For(implicitConversionExpression);
            }
        }

        public virtual void VisitImplicitOptionalConversionExpression(ImplicitOptionalConversionExpression implicitOptionalConversionExpression, A args)
        {
            VisitExpression(implicitOptionalConversionExpression.Expression, args);
        }

        public virtual void VisitImplicitNoneConversionExpression(ImplicitNoneConversionExpression implicitNoneConversionExpression, A args)
        {
            VisitExpression(implicitNoneConversionExpression.Expression, args);
        }

        public virtual void VisitImplicitImmutabilityConversionExpression(ImplicitImmutabilityConversionExpression implicitImmutabilityConversionExpression, A args)
        {
            VisitExpression(implicitImmutabilityConversionExpression.Expression, args);
        }

        public virtual void VisitImplicitStringLiteralConversionExpression(ImplicitStringLiteralConversionExpression implicitStringLiteralConversionExpression, A args)
        {
            VisitExpression((ExpressionSyntax)implicitStringLiteralConversionExpression.Expression, args);
        }

        public virtual void VisitImplicitNumericConversionExpression(ImplicitNumericConversionExpression implicitNumericConversionExpression, A args)
        {
            VisitExpression(implicitNumericConversionExpression.Expression, args);
        }

        public virtual void VisitBreakExpression(IBreakExpressionSyntax breakExpression, A args)
        {
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

        public virtual void VisitBlockOrResult(IBlockOrResultSyntax blockOrResult, A args)
        {
            switch (blockOrResult)
            {
                default:
                    throw ExhaustiveMatch.Failed(blockOrResult);
                case IBlockSyntax blockExpression:
                    VisitBlock(blockExpression, args);
                    break;
                case IResultStatementSyntax resultStatement:
                    VisitResultStatement(resultStatement, args);
                    break;
            }
        }

        public virtual void VisitElseClause(IElseClauseSyntax elseClause, A args)
        {
            switch (elseClause)
            {
                default:
                    throw ExhaustiveMatch.Failed(elseClause);
                case IBlockOrResultSyntax blockOrResult:
                    VisitBlockOrResult(blockOrResult, args);
                    break;
                case IIfExpressionSyntax ifExpression:
                    VisitIfExpression(ifExpression, args);
                    break;
            }
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

        public virtual void VisitMemberAccessExpression(MemberAccessExpressionSyntax memberAccessExpression, A args)
        {
            VisitExpression(memberAccessExpression.Expression, args);
        }

        public virtual void VisitNewObjectExpression(NewObjectExpressionSyntax newObjectExpression, A args)
        {
            VisitType(newObjectExpression.Constructor, args);
            foreach (var argument in newObjectExpression.Arguments)
                VisitArgument(argument, args);
        }

        public virtual void VisitArgument(IArgumentSyntax argument, A args)
        {
            VisitExpression(argument.Value, args);
        }

        public virtual void VisitReferenceLifetime(IReferenceLifetimeSyntax referenceLifetime, A args)
        {
            VisitType(referenceLifetime.ReferentType, args);
        }

        public virtual void VisitBlock(IBlockSyntax block, A args)
        {
            foreach (var statement in block.Statements)
                VisitStatement(statement, args);
        }

        public virtual void VisitAssignmentExpression(IAssignmentExpressionSyntax assignmentExpression, A args)
        {
            VisitExpression(assignmentExpression.LeftOperand, args);
            VisitExpression(assignmentExpression.RightOperand, args);
        }

        public virtual void VisitUnaryExpression(IUnaryExpressionSyntax unaryExpression, A args)
        {
            VisitExpression(unaryExpression.Operand, args);
        }

        public virtual void VisitName(INameSyntax name, A args)
        {
        }

        public virtual void VisitLiteralExpression(ILiteralExpressionSyntax literalExpression, A args)
        {
            // TODO this should dispatch on the type of literal
        }

        public virtual void VisitInvocation(InvocationSyntax invocation, A args)
        {
            switch (invocation)
            {
                default:
                    throw ExhaustiveMatch.Failed(invocation);
                case MethodInvocationSyntax methodInvocation:
                    VisitMethodInvocation(methodInvocation, args);
                    break;
                case AssociatedFunctionInvocationSyntax associatedFunctionInvocation:
                    VisitAssociatedFunctionInvocation(associatedFunctionInvocation, args);
                    break;
                case FunctionInvocationSyntax functionInvocation:
                    VisitFunctionInvocation(functionInvocation, args);
                    break;
            }
        }

        public virtual void VisitFunctionInvocation(FunctionInvocationSyntax functionInvocation, A args)
        {
            VisitName(functionInvocation.FunctionNameSyntax, args);
            foreach (var argument in functionInvocation.Arguments)
                VisitArgument(argument, args);
        }

        public virtual void VisitAssociatedFunctionInvocation(AssociatedFunctionInvocationSyntax associatedFunctionInvocation, A args)
        {
            foreach (var argument in associatedFunctionInvocation.Arguments)
                VisitArgument(argument, args);
        }

        public virtual void VisitMethodInvocation(MethodInvocationSyntax methodInvocation, A args)
        {
            VisitExpression(methodInvocation.Target, args);
            foreach (var argument in methodInvocation.Arguments)
                VisitArgument(argument, args);
        }

        public virtual void VisitReturnExpression(IReturnExpressionSyntax returnExpression, A args)
        {
            VisitExpression(returnExpression.ReturnValue, args);
        }

        public virtual void VisitBinaryExpression(IBinaryExpressionSyntax binaryExpression, A args)
        {
            VisitExpression(binaryExpression.LeftOperand, args);
            VisitExpression(binaryExpression.RightOperand, args);
        }
    }
}
