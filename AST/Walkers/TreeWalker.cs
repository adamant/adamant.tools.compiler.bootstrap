using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    internal class TreeWalker : ITreeWalker
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
            if (declaration == null || (declarationWalker?.ShouldSkip(declaration) ?? false))
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

        public void Walk(FixedList<IStatementSyntax>? statements)
        {
            if (statements == null) return;

            if (statementWalker?.Enter(statements, this) ?? true)
            {
                foreach (var statement in statements) Walk(statement);
                statementWalker?.Exit(statements, this);
            }
        }

        public void Walk(IStatementSyntax? statement)
        {
            if (statement == null) return;

            switch (statement)
            {
                default:
                    throw ExhaustiveMatch.Failed(statement);
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    if (statementWalker?.Enter(variableDeclaration, this) ?? true)
                    {
                        Walk(variableDeclaration.TypeSyntax);
                        Walk(variableDeclaration.Initializer);
                        statementWalker?.Exit(variableDeclaration, this);
                    }
                    break;
                case IExpressionStatementSyntax expressionStatement:
                    if (statementWalker?.Enter(expressionStatement, this) ?? true)
                    {
                        Walk(expressionStatement.Expression);
                        statementWalker?.Exit(expressionStatement, this);
                    }
                    break;
                case IResultStatementSyntax resultStatement:
                    if (statementWalker?.Enter(resultStatement, this) ?? true)
                    {
                        Walk(resultStatement.Expression);
                        statementWalker?.Exit(resultStatement, this);
                    }
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
                    if (statementWalker?.Enter(resultStatement, this) ?? true)
                    {
                        Walk(resultStatement.Expression);
                        statementWalker?.Exit(resultStatement, this);
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
                    if (expressionWalker?.Enter(ifExpression, this) ?? true)
                    {
                        Walk(ifExpression.Condition);
                        Walk(ifExpression.ThenBlock);
                        Walk(ifExpression.ElseClause);
                        expressionWalker?.Exit(ifExpression, this);
                    }
                    break;
            }
        }

        public void Walk(IBlockExpressionSyntax? blockExpression)
        {
            if (blockExpression == null) return;

            if (expressionWalker?.Enter(blockExpression, this) ?? true)
            {
                Walk(blockExpression.Statements);
                expressionWalker?.Exit(blockExpression, this);
            }
        }

        public void Walk(IExpressionSyntax? expression)
        {
            if (expression == null) return;

            switch (expression)
            {
                default:
                    throw new NotImplementedException();
                    throw ExhaustiveMatch.Failed(expression);
                case IUnsafeExpressionSyntax unsafeExpression:
                    if (expressionWalker?.Enter(unsafeExpression, this) ?? true)
                    {
                        Walk(unsafeExpression.Expression);
                        expressionWalker?.Exit(unsafeExpression, this);
                    }
                    break;
                case IBlockExpressionSyntax blockExpression:
                    if (expressionWalker?.Enter(blockExpression, this) ?? true)
                    {
                        foreach (var statement in blockExpression.Statements)
                            Walk(statement);
                        expressionWalker?.Exit(blockExpression, this);
                    }
                    break;
                case IFunctionInvocationExpressionSyntax functionInvocationExpression:
                    if (expressionWalker?.Enter(functionInvocationExpression, this) ?? true)
                    {
                        Walk(functionInvocationExpression.FunctionNameSyntax);
                        foreach (var argument in functionInvocationExpression.Arguments)
                            Walk(argument);
                        expressionWalker?.Exit(functionInvocationExpression, this);
                    }
                    break;
                case INameExpressionSyntax nameExpression:
                    if (expressionWalker?.Enter(nameExpression, this) ?? true)
                        expressionWalker?.Exit(nameExpression, this);
                    break;
                case IStringLiteralExpressionSyntax stringLiteralExpression:
                    if (expressionWalker?.Enter(stringLiteralExpression, this) ?? true)
                        expressionWalker?.Exit(stringLiteralExpression, this);
                    break;
                case IReturnExpressionSyntax returnExpression:
                    if (expressionWalker?.Enter(returnExpression, this) ?? true)
                    {
                        Walk(returnExpression.ReturnValue);
                        expressionWalker?.Exit(returnExpression, this);
                    }
                    break;
                case IIntegerLiteralExpressionSyntax integerLiteralExpression:
                    if (expressionWalker?.Enter(integerLiteralExpression, this) ?? true)
                        expressionWalker?.Exit(integerLiteralExpression, this);
                    break;
                case IMethodInvocationExpressionSyntax methodInvocationExpression:
                    if (expressionWalker?.Enter(methodInvocationExpression, this) ?? true)
                    {
                        Walk(methodInvocationExpression.Target);
                        Walk(methodInvocationExpression.MethodNameSyntax);
                        foreach (var argument in methodInvocationExpression.Arguments)
                            Walk(argument);
                        expressionWalker?.Exit(methodInvocationExpression, this);
                    }
                    break;
                case IAssignmentExpressionSyntax assignmentExpression:
                    if (expressionWalker?.Enter(assignmentExpression, this) ?? true)
                    {
                        Walk(assignmentExpression.LeftOperand);
                        Walk(assignmentExpression.RightOperand);
                        expressionWalker?.Exit(assignmentExpression, this);
                    }
                    break;
                case INewObjectExpressionSyntax newObjectExpression:
                    if (expressionWalker?.Enter(newObjectExpression, this) ?? true)
                    {
                        Walk(newObjectExpression.TypeSyntax);
                        Walk(newObjectExpression.ConstructorName);
                        foreach (var argument in newObjectExpression.Arguments)
                            Walk(argument);
                        expressionWalker?.Exit(newObjectExpression, this);
                    }
                    break;
                case IBoolLiteralExpressionSyntax boolLiteralExpression:
                    if (expressionWalker?.Enter(boolLiteralExpression, this) ?? true)
                        expressionWalker?.Exit(boolLiteralExpression, this);
                    break;
                case IIfExpressionSyntax ifExpression:
                    if (expressionWalker?.Enter(ifExpression, this) ?? true)
                    {
                        Walk(ifExpression.Condition);
                        Walk(ifExpression.ThenBlock);
                        Walk(ifExpression.ElseClause);
                        expressionWalker?.Exit(ifExpression, this);
                    }
                    break;
                case IBinaryOperatorExpressionSyntax binaryOperatorExpression:
                    if (expressionWalker?.Enter(binaryOperatorExpression, this) ?? true)
                    {
                        Walk(binaryOperatorExpression.LeftOperand);
                        Walk(binaryOperatorExpression.RightOperand);
                        expressionWalker?.Exit(binaryOperatorExpression, this);
                    }
                    break;
                case IUnaryOperatorExpressionSyntax unaryOperatorExpression:
                    if (expressionWalker?.Enter(unaryOperatorExpression, this) ?? true)
                    {
                        Walk(unaryOperatorExpression.Operand);
                        expressionWalker?.Exit(unaryOperatorExpression, this);
                    }
                    break;
                case ILoopExpressionSyntax loopExpression:
                    if (expressionWalker?.Enter(loopExpression, this) ?? true)
                    {
                        Walk(loopExpression.Block);
                        expressionWalker?.Exit(loopExpression, this);
                    }
                    break;
                case IWhileExpressionSyntax whileExpression:
                    if (expressionWalker?.Enter(whileExpression, this) ?? true)
                    {
                        Walk(whileExpression.Condition);
                        Walk(whileExpression.Block);
                        expressionWalker?.Exit(whileExpression, this);
                    }
                    break;
                case INoneLiteralExpressionSyntax noneLiteralExpression:
                    if (expressionWalker?.Enter(noneLiteralExpression, this) ?? true)
                        expressionWalker?.Exit(noneLiteralExpression, this);
                    break;
                case ISelfExpressionSyntax selfExpression:
                    if (expressionWalker?.Enter(selfExpression, this) ?? true)
                        expressionWalker?.Exit(selfExpression, this);
                    break;
                case INextExpressionSyntax nextExpression:
                    if (expressionWalker?.Enter(nextExpression, this) ?? true)
                        expressionWalker?.Exit(nextExpression, this);
                    break;
                case IMoveTransferSyntax moveExpression:
                    if (expressionWalker?.Enter(moveExpression, this) ?? true)
                    {
                        Walk(moveExpression.Expression);
                        expressionWalker?.Exit(moveExpression, this);
                    }
                    break;
                case IMemberAccessExpressionSyntax memberAccessExpression:
                    if (expressionWalker?.Enter(memberAccessExpression, this) ?? true)
                    {
                        Walk(memberAccessExpression.Expression);
                        Walk(memberAccessExpression.Member);
                        expressionWalker?.Exit(memberAccessExpression, this);
                    }
                    break;
                case IBreakExpressionSyntax breakExpression:
                    if (expressionWalker?.Enter(breakExpression, this) ?? true)
                    {
                        Walk(breakExpression.Value);
                        expressionWalker?.Exit(breakExpression, this);
                    }
                    break;
                case IForeachExpressionSyntax foreachExpression:
                    if (expressionWalker?.Enter(foreachExpression, this) ?? true)
                    {
                        Walk(foreachExpression.TypeSyntax);
                        Walk(foreachExpression.InExpression);
                        Walk(foreachExpression.Block);
                        expressionWalker?.Exit(foreachExpression, this);
                    }
                    break;
            }
        }

        public void Walk(ITransferSyntax? transfer)
        {
            if (transfer == null) return;

            switch (transfer)
            {
                default:
                    throw ExhaustiveMatch.Failed(transfer);
                case IImmutableTransferSyntax immutableTransfer:
                    if (expressionWalker?.Enter(immutableTransfer, this) ?? true)
                    {
                        Walk(immutableTransfer.Expression);
                        expressionWalker?.Exit(immutableTransfer, this);
                    }
                    break;
                case IMutableTransferSyntax mutableTransfer:
                    if (expressionWalker?.Enter(mutableTransfer, this) ?? true)
                    {
                        Walk(mutableTransfer.Expression);
                        expressionWalker?.Exit(mutableTransfer, this);
                    }
                    break;
                case IMoveTransferSyntax moveTransfer:
                    if (expressionWalker?.Enter(moveTransfer, this) ?? true)
                    {
                        Walk(moveTransfer.Expression);
                        expressionWalker?.Exit(moveTransfer, this);
                    }
                    break;
            }

        }
    }
}
