using System;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public static class SyntaxWalkerExtensions
    {
        public static void WalkChildren(this ISyntaxWalker walker, ISyntax syntax)
        {
            switch (syntax)
            {
                default:
                    throw new NotImplementedException(syntax.GetType().Name);
                    throw ExhaustiveMatch.Failed(syntax);
                case IClassDeclarationSyntax classDeclaration:
                    foreach (var member in classDeclaration.Members)
                        walker.Walk(member);
                    break;
                case IConstructorDeclarationSyntax constructorDeclaration:
                    foreach (var parameter in constructorDeclaration.Parameters)
                        walker.Walk(parameter);
                    walker.Walk(constructorDeclaration.Body);
                    break;
                case IConcreteMethodDeclarationSyntax concreteMethodDeclaration:
                    foreach (var parameter in concreteMethodDeclaration.Parameters)
                        walker.Walk(parameter);
                    walker.Walk(concreteMethodDeclaration.ReturnTypeSyntax);
                    walker.Walk(concreteMethodDeclaration.Body);
                    break;
                case IAbstractMethodDeclarationSyntax abstractMethodDeclaration:
                    foreach (var parameter in abstractMethodDeclaration.Parameters)
                        walker.Walk(parameter);
                    walker.Walk(abstractMethodDeclaration.ReturnTypeSyntax);
                    break;
                case IFunctionDeclarationSyntax functionDeclaration:
                    foreach (var parameter in functionDeclaration.Parameters)
                        walker.Walk(parameter);
                    walker.Walk(functionDeclaration.ReturnTypeSyntax);
                    walker.Walk(functionDeclaration.Body);
                    break;
                case IImplicitConversionExpression implicitConversion:
                    walker.Walk(implicitConversion.Expression);
                    break;
                case INamedParameterSyntax namedParameter:
                    walker.Walk(namedParameter.TypeSyntax);
                    walker.Walk(namedParameter.DefaultValue);
                    break;
                case IMutableTypeSyntax mutableType:
                    walker.Walk(mutableType.Referent);
                    break;
                case IReferenceLifetimeTypeSyntax referenceLifetimeType:
                    walker.Walk(referenceLifetimeType.ReferentType);
                    break;
                case IBodySyntax body:
                    foreach (var statement in body.Statements)
                        walker.Walk(statement);
                    break;
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    walker.Walk(variableDeclaration.TypeSyntax);
                    walker.Walk(variableDeclaration.Initializer);
                    break;
                case IExpressionStatementSyntax expressionStatement:
                    walker.Walk(expressionStatement.Expression);
                    break;
                case IResultStatementSyntax resultStatement:
                    walker.Walk(resultStatement.Expression);
                    break;
                case IImmutableTransferSyntax immutableTransfer:
                    walker.Walk(immutableTransfer.Expression);
                    break;
                case IMutableTransferSyntax mutableTransfer:
                    walker.Walk(mutableTransfer.Expression);
                    break;
                case IMoveTransferSyntax moveTransfer:
                    walker.Walk(moveTransfer.Expression);
                    break;
                case IIfExpressionSyntax ifExpression:
                    walker.Walk(ifExpression.Condition);
                    walker.Walk(ifExpression.ThenBlock);
                    walker.Walk(ifExpression.ElseClause);
                    break;
                case IUnsafeExpressionSyntax unsafeExpression:
                    walker.Walk(unsafeExpression.Expression);
                    break;
                case IBlockExpressionSyntax blockExpression:
                    foreach (var statement in blockExpression.Statements)
                        walker.Walk(statement);
                    break;
                case IFunctionInvocationExpressionSyntax functionInvocationExpression:
                    walker.Walk(functionInvocationExpression.FunctionNameSyntax);
                    foreach (var argument in functionInvocationExpression.Arguments)
                        walker.Walk(argument);
                    break;
                case IReturnExpressionSyntax returnExpression:
                    walker.Walk(returnExpression.ReturnValue);
                    break;
                case IMethodInvocationExpressionSyntax methodInvocationExpression:
                    walker.Walk(methodInvocationExpression.Target);
                    walker.Walk(methodInvocationExpression.MethodNameSyntax);
                    foreach (var argument in methodInvocationExpression.Arguments)
                        walker.Walk(argument);
                    break;
                case IAssignmentExpressionSyntax assignmentExpression:
                    walker.Walk(assignmentExpression.LeftOperand);
                    walker.Walk(assignmentExpression.RightOperand);
                    break;
                case INewObjectExpressionSyntax newObjectExpression:
                    walker.Walk(newObjectExpression.TypeSyntax);
                    walker.Walk(newObjectExpression.ConstructorName);
                    foreach (var argument in newObjectExpression.Arguments)
                        walker.Walk(argument);
                    break;
                case IBinaryOperatorExpressionSyntax binaryOperatorExpression:
                    walker.Walk(binaryOperatorExpression.LeftOperand);
                    walker.Walk(binaryOperatorExpression.RightOperand);
                    break;
                case IUnaryOperatorExpressionSyntax unaryOperatorExpression:
                    walker.Walk(unaryOperatorExpression.Operand);
                    break;
                case ILoopExpressionSyntax loopExpression:
                    walker.Walk(loopExpression.Block);
                    break;
                case IWhileExpressionSyntax whileExpression:
                    walker.Walk(whileExpression.Condition);
                    walker.Walk(whileExpression.Block);
                    break;
                case IMemberAccessExpressionSyntax memberAccessExpression:
                    walker.Walk(memberAccessExpression.Expression);
                    walker.Walk(memberAccessExpression.Member);
                    break;
                case IBreakExpressionSyntax breakExpression:
                    walker.Walk(breakExpression.Value);
                    break;
                case IForeachExpressionSyntax foreachExpression:
                    walker.Walk(foreachExpression.TypeSyntax);
                    walker.Walk(foreachExpression.InExpression);
                    walker.Walk(foreachExpression.Block);
                    break;
                case IBoolLiteralExpressionSyntax _:
                case IIntegerLiteralExpressionSyntax _:
                case IStringLiteralExpressionSyntax _:
                case INoneLiteralExpressionSyntax _:
                case ISelfExpressionSyntax _:
                case INextExpressionSyntax _:
                case ITypeNameSyntax _:
                case INameExpressionSyntax _:
                case ISelfParameterSyntax _:
                    // No Children
                    break;
            }
        }
    }
}
