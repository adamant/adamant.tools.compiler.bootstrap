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
            }
        }
    }
}
