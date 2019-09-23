using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public class ExpressionVisitor<A>
    {
        public virtual void VisitStatement(StatementSyntax statement, A args)
        {
            switch (statement)
            {
                case VariableDeclarationStatementSyntax variableDeclaration:
                    VisitVariableDeclarationStatement(variableDeclaration, args);
                    break;
                case ExpressionStatementSyntax expressionStatement:
                    VisitExpression(expressionStatement.Expression, args);
                    break;
                case null:
                    // Ignore
                    break;
                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        public virtual void VisitVariableDeclarationStatement(VariableDeclarationStatementSyntax variableDeclaration, A args)
        {
            VisitType(variableDeclaration.TypeSyntax, args);
            VisitExpression(variableDeclaration.Initializer, args);
        }

        public virtual void VisitExpression(ExpressionSyntax expression, A args)
        {
            switch (expression)
            {
                case BinaryExpressionSyntax binaryExpression:
                    VisitBinaryExpression(binaryExpression, args);
                    break;
                case ReturnExpressionSyntax returnExpression:
                    VisitReturnExpression(returnExpression, args);
                    break;
                case InvocationSyntax invocation:
                    VisitInvocation(invocation, args);
                    break;
                case LiteralExpressionSyntax literalExpression:
                    VisitLiteralExpression(literalExpression, args);
                    break;
                case NameSyntax identifierName:
                    VisitName(identifierName, args);
                    break;
                case UnaryExpressionSyntax unaryExpression:
                    VisitUnaryExpression(unaryExpression, args);
                    break;
                case AssignmentExpressionSyntax assignmentExpression:
                    VisitAssignmentExpression(assignmentExpression, args);
                    break;
                case BlockSyntax block:
                    VisitBlock(block, args);
                    break;
                case NewObjectExpressionSyntax newObjectExpression:
                    VisitNewObjectExpression(newObjectExpression, args);
                    break;
                case MemberAccessExpressionSyntax memberAccessExpression:
                    VisitMemberAccessExpression(memberAccessExpression, args);
                    break;
                case UnsafeExpressionSyntax unsafeExpression:
                    VisitUnsafeExpression(unsafeExpression, args);
                    break;
                case SelfExpressionSyntax selfExpression:
                    VisitSelfExpression(selfExpression, args);
                    break;
                case LoopExpressionSyntax loopExpression:
                    VisitLoopExpression(loopExpression, args);
                    break;
                case IfExpressionSyntax ifExpression:
                    VisitIfExpression(ifExpression, args);
                    break;
                //case ResultStatementSyntax resultExpression:
                //    VisitResultExpression(resultExpression, args);
                //    break;
                case BreakExpressionSyntax breakExpression:
                    VisitBreakExpression(breakExpression, args);
                    break;
                case ImplicitConversionExpression implicitConversion:
                    VisitImplicitConversionExpression(implicitConversion, args);
                    break;
                case MoveExpressionSyntax moveExpression:
                    VisitMoveExpression(moveExpression, args);
                    break;
                case ForeachExpressionSyntax foreachExpression:
                    VisitForeachExpression(foreachExpression, args);
                    break;
                case NextExpressionSyntax nextExpression:
                    VisitNextExpression(nextExpression, args);
                    break;
                case WhileExpressionSyntax whileExpression:
                    VisitWhileExpression(whileExpression, args);
                    break;
                case null:
                    // Ignore
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        public virtual void VisitType(TypeSyntax type, A args)
        {
            switch (type)
            {
                default:
                    throw ExhaustiveMatch.Failed(type);
                case null:
                    // Do nothing
                    break;
                case TypeNameSyntax typeName:
                    VisitTypeName(typeName, args);
                    break;
                case MutableTypeSyntax mutableType:
                    VisitMutableType(mutableType, args);
                    break;
                case ReferenceLifetimeSyntax referenceLifetime:
                    VisitReferenceLifetime(referenceLifetime, args);
                    break;
                case SelfTypeSyntax selfType:
                    VisitSelfType(selfType, args);
                    break;
            }
        }

        public virtual void VisitSelfType(SelfTypeSyntax selfType, A args)
        {

        }

        public virtual void VisitTypeName(TypeNameSyntax typeName, A args)
        {
        }

        public virtual void VisitWhileExpression(WhileExpressionSyntax whileExpression, A args)
        {
            VisitExpression(whileExpression.Condition, args);
            VisitExpression(whileExpression.Block, args);
        }

        public virtual void VisitNextExpression(NextExpressionSyntax nextExpression, A args)
        {
        }

        public virtual void VisitForeachExpression(ForeachExpressionSyntax foreachExpression, A args)
        {
            VisitType(foreachExpression.TypeSyntax, args);
            VisitExpression(foreachExpression.InExpression, args);
            VisitExpression(foreachExpression.Block, args);
        }

        public virtual void VisitMoveExpression(MoveExpressionSyntax moveExpression, A args)
        {
            VisitExpression(moveExpression.Expression, args);
        }

        public virtual void VisitMutableType(MutableTypeSyntax mutableType, A args)
        {
            throw new NotImplementedException();
        }

        public virtual void VisitImplicitConversionExpression(ImplicitConversionExpression implicitConversionExpression, A args)
        {
            switch (implicitConversionExpression)
            {
                case ImplicitNumericConversionExpression implicitNumericConversionExpression:
                    VisitImplicitNumericConversionExpression(implicitNumericConversionExpression, args);
                    break;
                case ImplicitLiteralConversionExpression implicitLiteralConversionExpression:
                    VisitImplicitLiteralConversionExpression(implicitLiteralConversionExpression, args);
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

        public virtual void VisitImplicitLiteralConversionExpression(ImplicitLiteralConversionExpression implicitLiteralConversionExpression, A args)
        {
            VisitExpression(implicitLiteralConversionExpression.Expression, args);
        }

        public virtual void VisitImplicitNumericConversionExpression(ImplicitNumericConversionExpression implicitNumericConversionExpression, A args)
        {
            VisitExpression(implicitNumericConversionExpression.Expression, args);
        }

        public virtual void VisitBreakExpression(BreakExpressionSyntax breakExpression, A args)
        {
        }

        public virtual void VisitResultStatement(ResultStatementSyntax resultStatement, A args)
        {
            VisitExpression(resultStatement.Expression, args);
        }

        public virtual void VisitIfExpression(IfExpressionSyntax ifExpression, A args)
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
                case BlockSyntax blockExpression:
                    VisitBlock(blockExpression, args);
                    break;
                case ResultStatementSyntax resultStatement:
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
                case IfExpressionSyntax ifExpression:
                    VisitIfExpression(ifExpression, args);
                    break;
            }
        }

        public virtual void VisitLoopExpression(LoopExpressionSyntax loopExpression, A args)
        {
            VisitExpression(loopExpression.Block, args);
        }

        public virtual void VisitSelfExpression(SelfExpressionSyntax selfExpression, A args)
        {
        }

        public virtual void VisitUnsafeExpression(UnsafeExpressionSyntax unsafeExpression, A args)
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

        public virtual void VisitArgument(ArgumentSyntax argument, A args)
        {
            VisitExpression(argument.Value, args);
        }

        public virtual void VisitReferenceLifetime(ReferenceLifetimeSyntax referenceLifetime, A args)
        {
            VisitType(referenceLifetime.ReferentType, args);
        }

        public virtual void VisitBlock(BlockSyntax block, A args)
        {
            foreach (var statement in block.Statements)
                VisitStatement(statement, args);
        }

        public virtual void VisitAssignmentExpression(AssignmentExpressionSyntax assignmentExpression, A args)
        {
            VisitExpression(assignmentExpression.LeftOperand, args);
            VisitExpression(assignmentExpression.RightOperand, args);
        }

        public virtual void VisitUnaryExpression(UnaryExpressionSyntax unaryExpression, A args)
        {
            VisitExpression(unaryExpression.Operand, args);
        }

        public virtual void VisitName(NameSyntax name, A args)
        {
        }

        public virtual void VisitLiteralExpression(LiteralExpressionSyntax literalExpression, A args)
        {
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

        public virtual void VisitReturnExpression(ReturnExpressionSyntax returnExpression, A args)
        {
            VisitExpression(returnExpression.ReturnValue, args);
        }

        public virtual void VisitBinaryExpression(BinaryExpressionSyntax binaryExpression, A args)
        {
            VisitExpression(binaryExpression.LeftOperand, args);
            VisitExpression(binaryExpression.RightOperand, args);
        }
    }
}
