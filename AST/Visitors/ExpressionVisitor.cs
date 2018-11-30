using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public class ExpressionVisitor<A>
    {
        public virtual void VisitStatement([CanBeNull] StatementSyntax statement, A args)
        {
            switch (statement)
            {
                case VariableDeclarationStatementSyntax variableDeclaration:
                    VisitVariableDeclarationStatement(variableDeclaration, args);
                    break;
                case ExpressionSyntax expression:
                    VisitExpression(expression, args);
                    break;
                case null:
                    // Ignore
                    break;
                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        public virtual void VisitVariableDeclarationStatement([NotNull] VariableDeclarationStatementSyntax variableDeclaration, A args)
        {
            VisitExpression(variableDeclaration.TypeExpression, args);
            VisitExpression(variableDeclaration.Initializer, args);
        }

        public virtual void VisitExpression([CanBeNull] ExpressionSyntax expression, A args)
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
                case IdentifierNameSyntax identifierName:
                    VisitIdentifierName(identifierName, args);
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
                case LifetimeTypeSyntax lifetimeType:
                    VisitLifetimeType(lifetimeType, args);
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
                case GenericsInvocationSyntax genericsInvocation:
                    VisitGenericInvocation(genericsInvocation, args);
                    break;
                case LifetimeNameSyntax lifetimeName:
                    VisitLifetimeName(lifetimeName, args);
                    break;
                case SelfExpressionSyntax selfExpression:
                    VisitSelfExpression(selfExpression, args);
                    break;
                case BaseExpressionSyntax baseExpression:
                    VisitBaseExpression(baseExpression, args);
                    break;
                case LoopExpressionSyntax loopExpression:
                    VisitLoopExpression(loopExpression, args);
                    break;
                case IfExpressionSyntax ifExpression:
                    VisitIfExpression(ifExpression, args);
                    break;
                case ResultExpressionSyntax resultExpression:
                    VisitResultExpression(resultExpression, args);
                    break;
                case BreakExpressionSyntax breakExpression:
                    VisitBreakExpression(breakExpression, args);
                    break;
                case ImplicitConversionExpression implicitConversion:
                    VisitImplicitConversionExpression(implicitConversion, args);
                    break;
                case null:
                    // Ignore
                    break;
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        public virtual void VisitImplicitConversionExpression([CanBeNull] ImplicitConversionExpression implicitConversionExpression, A args)
        {
            switch (implicitConversionExpression)
            {
                case ImplicitNumericConversionExpression implicitNumericConversionExpression:
                    VisitImplicitNumericConversionExpression(implicitNumericConversionExpression, args);
                    break;
                case ImplicitLiteralConversionExpression implicitLiteralConversionExpression:
                    VisitImplicitLiteralConversionExpression(implicitLiteralConversionExpression, args);
                    break;
                case null:
                    // Ignore
                    break;
                default:
                    throw NonExhaustiveMatchException.For(implicitConversionExpression);
            }
        }

        public virtual void VisitImplicitLiteralConversionExpression([NotNull] ImplicitLiteralConversionExpression implicitLiteralConversionExpression, A args)
        {
            VisitExpression(implicitLiteralConversionExpression.Expression, args);
        }

        public virtual void VisitImplicitNumericConversionExpression([NotNull] ImplicitNumericConversionExpression implicitNumericConversionExpression, A args)
        {
            VisitExpression(implicitNumericConversionExpression.Expression, args);
        }

        public virtual void VisitBreakExpression(BreakExpressionSyntax breakExpression, A args)
        {
        }

        public virtual void VisitResultExpression([NotNull] ResultExpressionSyntax resultExpression, A args)
        {
            VisitExpression(resultExpression.Expression, args);
        }

        public virtual void VisitIfExpression([NotNull] IfExpressionSyntax ifExpression, A args)
        {
            VisitExpression(ifExpression.Condition, args);
            VisitExpression(ifExpression.ThenBlock, args);
            VisitExpression(ifExpression.ElseClause, args);
        }

        public virtual void VisitLoopExpression([NotNull] LoopExpressionSyntax loopExpression, A args)
        {
            VisitExpression(loopExpression.Block, args);
        }

        public virtual void VisitBaseExpression(BaseExpressionSyntax baseExpression, A args)
        {
        }

        public virtual void VisitSelfExpression(SelfExpressionSyntax selfExpression, A args)
        {
        }

        public virtual void VisitLifetimeName(LifetimeNameSyntax lifetimeName, A args)
        {
        }

        public virtual void VisitGenericInvocation([NotNull] GenericsInvocationSyntax genericsInvocation, A args)
        {
            VisitExpression(genericsInvocation.Callee, args);
            foreach (var argument in genericsInvocation.Arguments) VisitArgument(argument, args);
        }

        public virtual void VisitUnsafeExpression([NotNull] UnsafeExpressionSyntax unsafeExpression, A args)
        {
            VisitExpression(unsafeExpression.Expression, args);
        }

        public virtual void VisitMemberAccessExpression([NotNull] MemberAccessExpressionSyntax memberAccessExpression, A args)
        {
            VisitExpression(memberAccessExpression.Expression, args);
        }

        public virtual void VisitNewObjectExpression([NotNull] NewObjectExpressionSyntax newObjectExpression, A args)
        {
            VisitExpression(newObjectExpression.Constructor, args);
            foreach (var argument in newObjectExpression.Arguments) VisitArgument(argument, args);
        }

        public virtual void VisitArgument([NotNull] ArgumentSyntax argument, A args)
        {
            VisitExpression(argument.Value, args);
        }

        public virtual void VisitLifetimeType([NotNull] LifetimeTypeSyntax lifetimeType, A args)
        {
            VisitExpression(lifetimeType.ReferentTypeExpression, args);
        }

        public virtual void VisitBlock([NotNull] BlockSyntax block, A args)
        {
            foreach (var statement in block.Statements) VisitStatement(statement, args);
        }

        public virtual void VisitAssignmentExpression([NotNull] AssignmentExpressionSyntax assignmentExpression, A args)
        {
            VisitExpression(assignmentExpression.LeftOperand, args);
            VisitExpression(assignmentExpression.RightOperand, args);
        }

        public virtual void VisitUnaryExpression([NotNull] UnaryExpressionSyntax unaryExpression, A args)
        {
            VisitExpression(unaryExpression.Operand, args);
        }

        public virtual void VisitIdentifierName([NotNull] IdentifierNameSyntax identifierName, A args)
        {
        }

        public virtual void VisitLiteralExpression([NotNull] LiteralExpressionSyntax literalExpression, A args)
        {
        }

        public virtual void VisitInvocation([NotNull] InvocationSyntax invocation, A args)
        {
            VisitExpression(invocation.Callee, args);
            foreach (var argument in invocation.Arguments) VisitArgument(argument, args);
        }

        public virtual void VisitReturnExpression([NotNull] ReturnExpressionSyntax returnExpression, A args)
        {
            VisitExpression(returnExpression.ReturnValue, args);
        }

        public virtual void VisitBinaryExpression([NotNull] BinaryExpressionSyntax binaryExpression, A args)
        {
            VisitExpression(binaryExpression.LeftOperand, args);
            VisitExpression(binaryExpression.RightOperand, args);
        }
    }
}
