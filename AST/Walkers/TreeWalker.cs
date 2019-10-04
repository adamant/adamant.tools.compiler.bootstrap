using System;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    internal class TreeWalker
    {
        private readonly IDeclarationWalker? declarationWalker;
        private readonly ITypeWalker? typeWalker;
        private readonly IStatementWalker? statementWalker;
        private readonly IExpressionWalker? expressionWalker;

        public TreeWalker(
            IDeclarationWalker? declarationWalker,
            ITypeWalker? typeWalker,
            IStatementWalker? statementWalker,
            IExpressionWalker? expressionWalker)
        {
            this.declarationWalker = declarationWalker;
            this.statementWalker = statementWalker;
            this.typeWalker = typeWalker;
            this.expressionWalker = expressionWalker;
        }

        public void Walk(IDeclarationSyntax? declaration)
        {
            if (declaration == null
               || (declarationWalker?.ShouldSkip(declaration) ?? false))
                return;

            switch (declaration)
            {
                default:
                    throw new NotImplementedException();
                    throw ExhaustiveMatch.Failed(declaration);
                case IClassDeclarationSyntax classDeclaration:
                    declarationWalker?.Enter(classDeclaration);
                    foreach (var member in classDeclaration.Members)
                        Walk(member);
                    declarationWalker?.Exit(classDeclaration);
                    break;
            }
        }

        public void Walk(ITypeSyntax? type)
        {
            if (typeWalker is null || type == null || typeWalker.ShouldSkip(type))
                return;

            switch (type)
            {
                default:
                    throw ExhaustiveMatch.Failed(type);
                case IMutableTypeSyntax mutableType:
                    typeWalker.Enter(mutableType);
                    Walk(mutableType.Referent);
                    typeWalker.Exit(mutableType);
                    break;
                case IReferenceLifetimeTypeSyntax referenceLifetimeType:
                    typeWalker.Enter(referenceLifetimeType);
                    Walk(referenceLifetimeType.ReferentType);
                    typeWalker.Exit(referenceLifetimeType);
                    break;
                case ITypeNameSyntax typeName:
                    typeWalker.Enter(typeName);
                    typeWalker.Exit(typeName);
                    break;
            }
        }

        public void Walk(IStatementSyntax? statement)
        {
            if (statement == null
                || (statementWalker?.ShouldSkip(statement) ?? false))
                return;

            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    statementWalker?.Enter(variableDeclaration);
                    Walk(variableDeclaration.TypeSyntax);
                    Walk(variableDeclaration.Initializer);
                    statementWalker?.Exit(variableDeclaration);
                    break;
                case IExpressionStatementSyntax expressionStatement:
                    statementWalker?.Enter(expressionStatement);
                    Walk(expressionStatement.Expression);
                    statementWalker?.Exit(expressionStatement);
                    break;
                case IResultStatementSyntax resultStatement:
                    statementWalker?.Enter(resultStatement);
                    Walk(resultStatement.Expression);
                    statementWalker?.Exit(resultStatement);
                    break;
            }
        }

        public void Walk(IBlockOrResultSyntax? blockOrResult)
        {
            if (blockOrResult == null) return;

            switch (blockOrResult)
            {
                default:
                    throw ExhaustiveMatch.Failed(blockOrResult);
                case IBlockExpressionSyntax blockExpression:
                    Walk(blockExpression);
                    break;
                case IResultStatementSyntax resultStatement:
                    if (!(statementWalker?.ShouldSkip(resultStatement) ?? false))
                    {
                        statementWalker?.Enter(resultStatement);
                        Walk(resultStatement.Expression);
                        statementWalker?.Exit(resultStatement);
                    }
                    break;
            }
        }

        public void Walk(IElseClauseSyntax? elseClause)
        {
            if (elseClause == null) return;

            switch (elseClause)
            {
                default:
                    throw ExhaustiveMatch.Failed(elseClause);
                case IBlockOrResultSyntax blockOrResult:
                    Walk(blockOrResult);
                    break;
                case IIfExpressionSyntax ifExpression:
                    if (!(expressionWalker?.ShouldSkip(ifExpression) ?? false))
                    {
                        expressionWalker?.Enter(ifExpression);
                        Walk(ifExpression.Condition);
                        Walk(ifExpression.ThenBlock);
                        Walk(ifExpression.ElseClause);
                        expressionWalker?.Exit(ifExpression);
                    }
                    break;
            }
        }

        public void Walk(IBlockExpressionSyntax? blockExpression)
        {
            if (blockExpression == null || (expressionWalker?.ShouldSkip(blockExpression) ?? false)) return;

            expressionWalker?.Enter(blockExpression);
            foreach (var statement in blockExpression.Statements) Walk(statement);
            expressionWalker?.Exit(blockExpression);
        }

        public void Walk(IExpressionSyntax? expression)
        {
            if (expression == null || (expressionWalker?.ShouldSkip(expression) ?? false))
                return;

            switch (expression)
            {
                default:
                    throw new NotImplementedException();
                    throw ExhaustiveMatch.Failed(expression);
                case IUnsafeExpressionSyntax unsafeExpression:
                    expressionWalker?.Enter(unsafeExpression);
                    Walk(unsafeExpression.Expression);
                    expressionWalker?.Exit(unsafeExpression);
                    break;
                case IBlockExpressionSyntax blockExpression:
                    expressionWalker?.Enter(blockExpression);
                    foreach (var statement in blockExpression.Statements)
                        Walk(statement);
                    expressionWalker?.Exit(blockExpression);
                    break;
                case IFunctionInvocationExpressionSyntax functionInvocationExpression:
                    expressionWalker?.Enter(functionInvocationExpression);
                    Walk(functionInvocationExpression.FunctionNameSyntax);
                    foreach (var argument in functionInvocationExpression.Arguments)
                        Walk(argument.Value);
                    expressionWalker?.Exit(functionInvocationExpression);
                    break;
                case INameExpressionSyntax nameExpression:
                    expressionWalker?.Enter(nameExpression);
                    expressionWalker?.Exit(nameExpression);
                    break;
                case IStringLiteralExpressionSyntax stringLiteralExpression:
                    expressionWalker?.Enter(stringLiteralExpression);
                    expressionWalker?.Exit(stringLiteralExpression);
                    break;
                case IReturnExpressionSyntax returnExpression:
                    expressionWalker?.Enter(returnExpression);
                    Walk(returnExpression.ReturnValue);
                    expressionWalker?.Exit(returnExpression);
                    break;
                case IIntegerLiteralExpressionSyntax integerLiteralExpression:
                    expressionWalker?.Enter(integerLiteralExpression);
                    expressionWalker?.Exit(integerLiteralExpression);
                    break;
                case IMethodInvocationExpressionSyntax methodInvocationExpression:
                    expressionWalker?.Enter(methodInvocationExpression);
                    Walk(methodInvocationExpression.Target);
                    Walk(methodInvocationExpression.MethodNameSyntax);
                    foreach (var argument in methodInvocationExpression.Arguments)
                        Walk(argument.Value);
                    expressionWalker?.Exit(methodInvocationExpression);
                    break;
                case IAssignmentExpressionSyntax assignmentExpression:
                    expressionWalker?.Enter(assignmentExpression);
                    Walk(assignmentExpression.LeftOperand);
                    Walk(assignmentExpression.RightOperand);
                    expressionWalker?.Exit(assignmentExpression);
                    break;
                case INewObjectExpressionSyntax newObjectExpression:
                    expressionWalker?.Enter(newObjectExpression);
                    Walk(newObjectExpression.TypeSyntax);
                    Walk(newObjectExpression.ConstructorName);
                    foreach (var argument in newObjectExpression.Arguments)
                        Walk(argument.Value);
                    expressionWalker?.Exit(newObjectExpression);
                    break;
                case IBoolLiteralExpressionSyntax boolLiteralExpression:
                    expressionWalker?.Enter(boolLiteralExpression);
                    expressionWalker?.Exit(boolLiteralExpression);
                    break;
                case IIfExpressionSyntax ifExpression:
                    expressionWalker?.Enter(ifExpression);
                    Walk(ifExpression.Condition);
                    Walk(ifExpression.ThenBlock);
                    Walk(ifExpression.ElseClause);
                    expressionWalker?.Exit(ifExpression);
                    break;
                case IBinaryOperatorExpressionSyntax binaryOperatorExpression:
                    expressionWalker?.Enter(binaryOperatorExpression);
                    Walk(binaryOperatorExpression.LeftOperand);
                    Walk(binaryOperatorExpression.RightOperand);
                    expressionWalker?.Exit(binaryOperatorExpression);
                    break;
                case IUnaryOperatorExpressionSyntax unaryOperatorExpression:
                    expressionWalker?.Enter(unaryOperatorExpression);
                    Walk(unaryOperatorExpression.Operand);
                    expressionWalker?.Exit(unaryOperatorExpression);
                    break;
                case ILoopExpressionSyntax loopExpression:
                    expressionWalker?.Enter(loopExpression);
                    Walk(loopExpression.Block);
                    expressionWalker?.Exit(loopExpression);
                    break;
                case IWhileExpressionSyntax whileExpression:
                    expressionWalker?.Enter(whileExpression);
                    Walk(whileExpression.Condition);
                    Walk(whileExpression.Block);
                    expressionWalker?.Exit(whileExpression);
                    break;
                case INoneLiteralExpressionSyntax noneLiteralExpression:
                    expressionWalker?.Enter(noneLiteralExpression);
                    expressionWalker?.Exit(noneLiteralExpression);
                    break;
                case ISelfExpressionSyntax selfExpression:
                    expressionWalker?.Enter(selfExpression);
                    expressionWalker?.Exit(selfExpression);
                    break;
                case INextExpressionSyntax nextExpression:
                    expressionWalker?.Enter(nextExpression);
                    expressionWalker?.Exit(nextExpression);
                    break;
                case IMoveExpressionSyntax moveExpression:
                    expressionWalker?.Enter(moveExpression);
                    Walk(moveExpression.Expression);
                    expressionWalker?.Exit(moveExpression);
                    break;
                case IMemberAccessExpressionSyntax memberAccessExpression:
                    expressionWalker?.Enter(memberAccessExpression);
                    Walk(memberAccessExpression.Expression);
                    Walk(memberAccessExpression.Member);
                    expressionWalker?.Exit(memberAccessExpression);
                    break;
                case IBreakExpressionSyntax breakExpression:
                    expressionWalker?.Enter(breakExpression);
                    Walk(breakExpression.Value);
                    expressionWalker?.Exit(breakExpression);
                    break;
                case IForeachExpressionSyntax foreachExpression:
                    expressionWalker?.Enter(foreachExpression);
                    Walk(foreachExpression.TypeSyntax);
                    Walk(foreachExpression.InExpression);
                    Walk(foreachExpression.Block);
                    expressionWalker?.Exit(foreachExpression);
                    break;
            }
        }
    }
}
