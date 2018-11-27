using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public class ExpressionVisitor<A, R>
    {
        public virtual R DefaultResult(A args) => default;

        public virtual R CombineResults(A result, params R[] results) => default;

        public virtual R VisitStatement([CanBeNull] StatementSyntax statement, A args)
        {
            switch (statement)
            {
                case VariableDeclarationStatementSyntax variableDeclaration:
                    return VisitVariableDeclarationStatement(variableDeclaration, args);
                case ExpressionSyntax expression:
                    return VisitExpression(expression, args);
                default:
                    throw NonExhaustiveMatchException.For(statement);
            }
        }

        public virtual R VisitVariableDeclarationStatement([NotNull] VariableDeclarationStatementSyntax variableDeclaration, A args)
        {
            var r1 = VisitExpression(variableDeclaration.TypeExpression, args);
            var r2 = VisitExpression(variableDeclaration.Initializer, args);
            return CombineResults(args, r1, r2);
        }

        public virtual R VisitExpression([CanBeNull] ExpressionSyntax expression, A args)
        {
            switch (expression)
            {
                case BinaryExpressionSyntax binaryExpression:
                    return VisitBinaryExpression(binaryExpression, args);
                case ReturnExpressionSyntax returnExpression:
                    return VisitReturnExpression(returnExpression, args);
                case InvocationSyntax invocation:
                    return VisitInvocation(invocation, args);
                case LiteralExpressionSyntax literalExpression:
                    return VisitLiteralExpression(literalExpression, args);
                case IdentifierNameSyntax identifierName:
                    return VisitIdentifierName(identifierName, args);
                case UnaryExpressionSyntax unaryExpression:
                    return VisitUnaryExpression(unaryExpression, args);
                case PrimitiveTypeSyntax primitiveType:
                    return VisitPrimitiveType(primitiveType, args);
                case AssignmentExpressionSyntax assignmentExpression:
                    return VisitAssignmentExpression(assignmentExpression, args);
                case BlockSyntax block:
                    return VisitBlock(block, args);
                case LifetimeTypeSyntax lifetimeType:
                    return VisitLifetimeType(lifetimeType, args);
                case NewObjectExpressionSyntax newObjectExpression:
                    return VisitNewObjectExpression(newObjectExpression, args);
                case MemberAccessExpressionSyntax memberAccessExpression:
                    return VisitMemberAccessExpression(memberAccessExpression, args);
                case UnsafeExpressionSyntax unsafeExpression:
                    return VisitUnsafeExpression(unsafeExpression, args);
                case GenericsInvocationSyntax genericsInvocation:
                    return VisitGenericInvocation(genericsInvocation, args);
                case LifetimeNameSyntax lifetimeName:
                    return VisitLifetimeName(lifetimeName, args);
                case SelfExpressionSyntax selfExpression:
                    return VisitSelfExpression(selfExpression, args);
                case BaseExpressionSyntax baseExpression:
                    return VisitBaseExpression(baseExpression, args);
                case null:
                    return VisitNull(args);
                default:
                    throw NonExhaustiveMatchException.For(expression);
            }
        }

        public virtual R VisitBaseExpression(BaseExpressionSyntax baseExpression, A args)
        {
            return DefaultResult(args);
        }

        public virtual R VisitSelfExpression(SelfExpressionSyntax selfExpression, A args)
        {
            return DefaultResult(args);
        }

        public virtual R VisitLifetimeName(LifetimeNameSyntax lifetimeName, A args)
        {
            return DefaultResult(args);
        }

        public virtual R VisitGenericInvocation([NotNull] GenericsInvocationSyntax genericsInvocation, A args)
        {
            var r = VisitExpression(genericsInvocation.Callee, args);
            var argumentResults = genericsInvocation.Arguments.Select(a => VisitExpression(a.Value, args));
            return CombineResults(args, r.Yield().Concat(argumentResults).ToArray());
        }

        public virtual R VisitUnsafeExpression([NotNull] UnsafeExpressionSyntax unsafeExpression, A args)
        {
            return VisitExpression(unsafeExpression.Expression, args);
        }

        public virtual R VisitMemberAccessExpression([NotNull] MemberAccessExpressionSyntax memberAccessExpression, A args)
        {
            return VisitExpression(memberAccessExpression.Expression, args);
        }

        public virtual R VisitNewObjectExpression([NotNull] NewObjectExpressionSyntax newObjectExpression, A args)
        {
            var r = VisitExpression(newObjectExpression.Constructor, args);
            var argumentResults = newObjectExpression.Arguments.Select(a => VisitExpression(a.Value, args));
            return CombineResults(args, r.Yield().Concat(argumentResults).ToArray());
        }

        public virtual R VisitLifetimeType([NotNull] LifetimeTypeSyntax lifetimeType, A args)
        {
            return VisitExpression(lifetimeType.TypeExpression, args);
        }

        public virtual R VisitBlock([NotNull] BlockSyntax block, A args)
        {
            return CombineResults(args, block.Statements.Select(s => VisitStatement(s, args)).ToArray());
        }

        public virtual R VisitAssignmentExpression([NotNull] AssignmentExpressionSyntax assignmentExpression, A args)
        {
            var r1 = VisitExpression(assignmentExpression.LeftOperand, args);
            var r2 = VisitExpression(assignmentExpression.RightOperand, args);
            return CombineResults(args, r1, r2);
        }

        public virtual R VisitPrimitiveType(PrimitiveTypeSyntax primitiveType, A args)
        {
            return DefaultResult(args);
        }

        public virtual R VisitUnaryExpression([NotNull] UnaryExpressionSyntax unaryExpression, A args)
        {
            return VisitExpression(unaryExpression.Operand, args);
        }

        public virtual R VisitIdentifierName([NotNull] IdentifierNameSyntax identifierName, A args)
        {
            return DefaultResult(args);
        }

        public virtual R VisitNull(A args)
        {
            return DefaultResult(args);
        }

        public virtual R VisitLiteralExpression([NotNull] LiteralExpressionSyntax literalExpression, A args)
        {
            return DefaultResult(args);
        }

        public virtual R VisitInvocation([NotNull] InvocationSyntax invocation, A args)
        {
            var r = VisitExpression(invocation.Callee, args);
            var argumentResults = invocation.Arguments.Select(a => VisitExpression(a.Value, args));
            return CombineResults(args, r.Yield().Concat(argumentResults).ToArray());
        }

        public virtual R VisitReturnExpression([NotNull] ReturnExpressionSyntax returnExpression, A args)
        {
            return VisitExpression(returnExpression.ReturnValue, args);
        }

        public virtual R VisitBinaryExpression([NotNull] BinaryExpressionSyntax binaryExpression, A args)
        {
            var r1 = VisitExpression(binaryExpression.LeftOperand, args);
            var r2 = VisitExpression(binaryExpression.RightOperand, args);
            return CombineResults(args, r1, r2);
        }
    }
}
