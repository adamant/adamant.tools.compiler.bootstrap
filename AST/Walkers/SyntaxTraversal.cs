using System;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    internal class SyntaxTraversal : ISyntaxTraversal
    {
        private readonly ISyntaxWalker walker;

        public SyntaxTraversal(ISyntaxWalker walker)
        {
            this.walker = walker;
        }

        public void Walk(ISyntax? syntax)
        {
            if (syntax == null) return;
            switch (syntax)
            {
                default:
                    throw new NotImplementedException(syntax.GetType().Name);
                    throw ExhaustiveMatch.Failed(syntax);
                case IClassDeclarationSyntax classDeclaration:
                    if (walker.Enter(classDeclaration, this))
                        foreach (var member in classDeclaration.Members)
                            Walk(member);
                    walker.Exit(classDeclaration, this);
                    break;
                case IMutableTypeSyntax mutableType:
                    if (walker.Enter(mutableType, this))
                        Walk(mutableType.Referent);
                    walker.Exit(mutableType, this);
                    break;
                case IReferenceLifetimeTypeSyntax referenceLifetimeType:
                    if (walker.Enter(referenceLifetimeType, this))
                        Walk(referenceLifetimeType.ReferentType);
                    walker.Exit(referenceLifetimeType, this);
                    break;
                case ITypeNameSyntax typeName:
                    walker.Enter(typeName, this);
                    walker.Exit(typeName, this);
                    break;
                case IBodySyntax body:
                    if (walker.Enter(body, this))
                        foreach (var statement in body.Statements)
                            Walk(statement);
                    walker.Exit(body, this);
                    break;
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    if (walker.Enter(variableDeclaration, this))
                    {
                        Walk(variableDeclaration.TypeSyntax);
                        Walk(variableDeclaration.Initializer);
                    }
                    walker.Exit(variableDeclaration, this);
                    break;
                case IExpressionStatementSyntax expressionStatement:
                    if (walker.Enter(expressionStatement, this))
                        Walk(expressionStatement.Expression);
                    walker.Exit(expressionStatement, this);
                    break;
                case IResultStatementSyntax resultStatement:
                    if (walker.Enter(resultStatement, this))
                        Walk(resultStatement.Expression);
                    walker.Exit(resultStatement, this);
                    break;
                case IImmutableTransferSyntax immutableTransfer:
                    if (walker.Enter(immutableTransfer, this))
                        Walk(immutableTransfer.Expression);
                    walker.Exit(immutableTransfer, this);
                    break;
                case IMutableTransferSyntax mutableTransfer:
                    if (walker.Enter(mutableTransfer, this))
                        Walk(mutableTransfer.Expression);
                    walker.Exit(mutableTransfer, this);
                    break;
                case IMoveTransferSyntax moveTransfer:
                    if (walker.Enter(moveTransfer, this))
                        Walk(moveTransfer.Expression);
                    walker.Exit(moveTransfer, this);
                    break;
                case IIfExpressionSyntax ifExpression:
                    if (walker.Enter(ifExpression, this))
                    {
                        Walk(ifExpression.Condition);
                        Walk(ifExpression.ThenBlock);
                        Walk(ifExpression.ElseClause);
                    }
                    walker.Exit(ifExpression, this);
                    break;
                case IUnsafeExpressionSyntax unsafeExpression:
                    if (walker.Enter(unsafeExpression, this))
                        Walk(unsafeExpression.Expression);
                    walker.Exit(unsafeExpression, this);
                    break;
                case IBlockExpressionSyntax blockExpression:
                    if (walker.Enter(blockExpression, this))
                        foreach (var statement in blockExpression.Statements)
                            Walk(statement);
                    walker.Exit(blockExpression, this);
                    break;
                case IFunctionInvocationExpressionSyntax functionInvocationExpression:
                    if (walker.Enter(functionInvocationExpression, this))
                    {
                        Walk(functionInvocationExpression.FunctionNameSyntax);
                        foreach (var argument in functionInvocationExpression.Arguments)
                            Walk(argument);
                    }
                    walker.Exit(functionInvocationExpression, this);
                    break;
                case INameExpressionSyntax nameExpression:
                    walker.Enter(nameExpression, this);
                    walker.Exit(nameExpression, this);
                    break;
                case IStringLiteralExpressionSyntax stringLiteralExpression:
                    walker.Enter(stringLiteralExpression, this);
                    walker.Exit(stringLiteralExpression, this);
                    break;
                case IReturnExpressionSyntax returnExpression:
                    if (walker.Enter(returnExpression, this))
                        Walk(returnExpression.ReturnValue);
                    walker.Exit(returnExpression, this);
                    break;
                case IIntegerLiteralExpressionSyntax integerLiteralExpression:
                    walker.Enter(integerLiteralExpression, this);
                    walker.Exit(integerLiteralExpression, this);
                    break;
                case IMethodInvocationExpressionSyntax methodInvocationExpression:
                    if (walker.Enter(methodInvocationExpression, this))
                    {
                        Walk(methodInvocationExpression.Target);
                        Walk(methodInvocationExpression.MethodNameSyntax);
                        foreach (var argument in methodInvocationExpression.Arguments)
                            Walk(argument);
                    }
                    walker.Exit(methodInvocationExpression, this);
                    break;
                case IAssignmentExpressionSyntax assignmentExpression:
                    if (walker.Enter(assignmentExpression, this))
                    {
                        Walk(assignmentExpression.LeftOperand);
                        Walk(assignmentExpression.RightOperand);
                    }
                    walker.Exit(assignmentExpression, this);
                    break;
                case INewObjectExpressionSyntax newObjectExpression:
                    if (walker.Enter(newObjectExpression, this))
                    {
                        Walk(newObjectExpression.TypeSyntax);
                        Walk(newObjectExpression.ConstructorName);
                        foreach (var argument in newObjectExpression.Arguments)
                            Walk(argument);
                    }
                    walker.Exit(newObjectExpression, this);
                    break;
                case IBoolLiteralExpressionSyntax boolLiteralExpression:
                    if (walker.Enter(boolLiteralExpression, this))
                        walker.Exit(boolLiteralExpression, this);
                    break;
                case IBinaryOperatorExpressionSyntax binaryOperatorExpression:
                    if (walker.Enter(binaryOperatorExpression, this))
                    {
                        Walk(binaryOperatorExpression.LeftOperand);
                        Walk(binaryOperatorExpression.RightOperand);
                    }
                    walker.Exit(binaryOperatorExpression, this);
                    break;
                case IUnaryOperatorExpressionSyntax unaryOperatorExpression:
                    if (walker.Enter(unaryOperatorExpression, this))
                        Walk(unaryOperatorExpression.Operand);
                    walker.Exit(unaryOperatorExpression, this);
                    break;
                case ILoopExpressionSyntax loopExpression:
                    if (walker.Enter(loopExpression, this))
                        Walk(loopExpression.Block);
                    walker.Exit(loopExpression, this);
                    break;
                case IWhileExpressionSyntax whileExpression:
                    if (walker.Enter(whileExpression, this))
                    {
                        Walk(whileExpression.Condition);
                        Walk(whileExpression.Block);
                    }
                    walker.Exit(whileExpression, this);
                    break;
                case INoneLiteralExpressionSyntax noneLiteralExpression:
                    walker.Enter(noneLiteralExpression, this);
                    walker.Exit(noneLiteralExpression, this);
                    break;
                case ISelfExpressionSyntax selfExpression:
                    walker.Enter(selfExpression, this);
                    walker.Exit(selfExpression, this);
                    break;
                case INextExpressionSyntax nextExpression:
                    walker.Enter(nextExpression, this);
                    walker.Exit(nextExpression, this);
                    break;
                case IMemberAccessExpressionSyntax memberAccessExpression:
                    if (walker.Enter(memberAccessExpression, this))
                    {
                        Walk(memberAccessExpression.Expression);
                        Walk(memberAccessExpression.Member);
                    }
                    walker.Exit(memberAccessExpression, this);
                    break;
                case IBreakExpressionSyntax breakExpression:
                    if (walker.Enter(breakExpression, this))
                        Walk(breakExpression.Value);
                    walker.Exit(breakExpression, this);
                    break;
                case IForeachExpressionSyntax foreachExpression:
                    if (walker.Enter(foreachExpression, this))
                    {
                        Walk(foreachExpression.TypeSyntax);
                        Walk(foreachExpression.InExpression);
                        Walk(foreachExpression.Block);
                    }
                    walker.Exit(foreachExpression, this);
                    break;
            }
        }
    }
}
