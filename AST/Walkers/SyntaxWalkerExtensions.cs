using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public static class SyntaxWalkerExtensions
    {
        public static void WalkChildren<T>(this ISyntaxWalker<T> walker, ISyntax syntax, T arg)
        {
            switch (syntax)
            {
                default:
                    throw ExhaustiveMatch.Failed(syntax);
                case ICompilationUnitSyntax compilationUnit:
                    foreach (var usingDirective in compilationUnit.UsingDirectives)
                        walker.Walk(usingDirective, arg);
                    foreach (var declaration in compilationUnit.Declarations)
                        walker.Walk(declaration, arg);
                    break;
                case INamespaceDeclarationSyntax namespaceDeclaration:
                    foreach (var usingDirective in namespaceDeclaration.UsingDirectives)
                        walker.Walk(usingDirective, arg);
                    foreach (var declaration in namespaceDeclaration.Declarations)
                        walker.Walk(declaration, arg);
                    break;
                case IClassDeclarationSyntax classDeclaration:
                    foreach (var member in classDeclaration.Members)
                        walker.Walk(member, arg);
                    break;
                case IConstructorDeclarationSyntax constructorDeclaration:
                    foreach (var parameter in constructorDeclaration.Parameters)
                        walker.Walk(parameter, arg);
                    walker.Walk(constructorDeclaration.Body, arg);
                    break;
                case IConcreteMethodDeclarationSyntax concreteMethodDeclaration:
                    foreach (var parameter in concreteMethodDeclaration.Parameters)
                        walker.Walk(parameter, arg);
                    walker.Walk(concreteMethodDeclaration.ReturnTypeSyntax, arg);
                    walker.Walk(concreteMethodDeclaration.Body, arg);
                    break;
                case IAbstractMethodDeclarationSyntax abstractMethodDeclaration:
                    foreach (var parameter in abstractMethodDeclaration.Parameters)
                        walker.Walk(parameter, arg);
                    walker.Walk(abstractMethodDeclaration.ReturnTypeSyntax, arg);
                    break;
                case IAssociatedFunctionDeclaration associatedFunctionDeclaration:
                    foreach (var parameter in associatedFunctionDeclaration.Parameters)
                        walker.Walk(parameter, arg);
                    walker.Walk(associatedFunctionDeclaration.ReturnTypeSyntax, arg);
                    walker.Walk(associatedFunctionDeclaration.Body, arg);
                    break;
                case IFunctionDeclarationSyntax functionDeclaration:
                    foreach (var parameter in functionDeclaration.Parameters)
                        walker.Walk(parameter, arg);
                    walker.Walk(functionDeclaration.ReturnTypeSyntax, arg);
                    walker.Walk(functionDeclaration.Body, arg);
                    break;
                case IFieldDeclarationSyntax fieldDeclaration:
                    walker.Walk(fieldDeclaration.TypeSyntax, arg);
                    walker.Walk(fieldDeclaration.Initializer, arg);
                    break;
                case IArgumentSyntax argument:
                    walker.Walk(argument.Expression, arg);
                    break;
                case IImplicitConversionExpression implicitConversion:
                    walker.Walk(implicitConversion.Expression, arg);
                    break;
                case INamedParameterSyntax namedParameter:
                    walker.Walk(namedParameter.TypeSyntax, arg);
                    walker.Walk(namedParameter.DefaultValue, arg);
                    break;
                case IFieldParameterSyntax fieldParameter:
                    walker.Walk(fieldParameter.DefaultValue, arg);
                    break;
                case IMutableTypeSyntax mutableType:
                    walker.Walk(mutableType.Referent, arg);
                    break;
                case IOptionalTypeSyntax optionalType:
                    walker.Walk(optionalType.Referent, arg);
                    break;
                case IReferenceLifetimeTypeSyntax referenceLifetimeType:
                    walker.Walk(referenceLifetimeType.ReferentType, arg);
                    break;
                case IBodySyntax body:
                    foreach (var statement in body.Statements)
                        walker.Walk(statement, arg);
                    break;
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    walker.Walk(variableDeclaration.TypeSyntax, arg);
                    walker.Walk(variableDeclaration.Initializer, arg);
                    break;
                case IExpressionStatementSyntax expressionStatement:
                    walker.Walk(expressionStatement.Expression, arg);
                    break;
                case IResultStatementSyntax resultStatement:
                    walker.Walk(resultStatement.Expression, arg);
                    break;
                case IMutableExpressionSyntax mutableTransfer:
                    walker.Walk(mutableTransfer.Referent, arg);
                    break;
                case IMoveExpressionSyntax moveTransfer:
                    walker.Walk(moveTransfer.Referent, arg);
                    break;
                case IIfExpressionSyntax ifExpression:
                    walker.Walk(ifExpression.Condition, arg);
                    walker.Walk(ifExpression.ThenBlock, arg);
                    walker.Walk(ifExpression.ElseClause, arg);
                    break;
                case IUnsafeExpressionSyntax unsafeExpression:
                    walker.Walk(unsafeExpression.Expression, arg);
                    break;
                case IBlockExpressionSyntax blockExpression:
                    foreach (var statement in blockExpression.Statements)
                        walker.Walk(statement, arg);
                    break;
                case IFunctionInvocationExpressionSyntax functionInvocationExpression:
                    walker.Walk(functionInvocationExpression.FunctionNameSyntax, arg);
                    foreach (var argument in functionInvocationExpression.Arguments)
                        walker.Walk(argument, arg);
                    break;
                case IAssociatedFunctionInvocationExpressionSyntax associatedFunctionInvocation:
                    walker.Walk(associatedFunctionInvocation.FunctionNameSyntax, arg);
                    foreach (var argument in associatedFunctionInvocation.Arguments)
                        walker.Walk(argument, arg);
                    break;
                case IReturnExpressionSyntax returnExpression:
                    walker.Walk(returnExpression.ReturnValue, arg);
                    break;
                case IMethodInvocationExpressionSyntax methodInvocationExpression:
                    walker.Walk(methodInvocationExpression.Target, arg);
                    walker.Walk(methodInvocationExpression.MethodNameSyntax, arg);
                    foreach (var argument in methodInvocationExpression.Arguments)
                        walker.Walk(argument, arg);
                    break;
                case IAssignmentExpressionSyntax assignmentExpression:
                    walker.Walk(assignmentExpression.LeftOperand, arg);
                    walker.Walk(assignmentExpression.RightOperand, arg);
                    break;
                case INewObjectExpressionSyntax newObjectExpression:
                    walker.Walk(newObjectExpression.TypeSyntax, arg);
                    walker.Walk(newObjectExpression.ConstructorName, arg);
                    foreach (var argument in newObjectExpression.Arguments)
                        walker.Walk(argument, arg);
                    break;
                case IBinaryOperatorExpressionSyntax binaryOperatorExpression:
                    walker.Walk(binaryOperatorExpression.LeftOperand, arg);
                    walker.Walk(binaryOperatorExpression.RightOperand, arg);
                    break;
                case IUnaryOperatorExpressionSyntax unaryOperatorExpression:
                    walker.Walk(unaryOperatorExpression.Operand, arg);
                    break;
                case ILoopExpressionSyntax loopExpression:
                    walker.Walk(loopExpression.Block, arg);
                    break;
                case IWhileExpressionSyntax whileExpression:
                    walker.Walk(whileExpression.Condition, arg);
                    walker.Walk(whileExpression.Block, arg);
                    break;
                case IMemberAccessExpressionSyntax memberAccessExpression:
                    walker.Walk(memberAccessExpression.Expression, arg);
                    walker.Walk(memberAccessExpression.Member, arg);
                    break;
                case IBreakExpressionSyntax breakExpression:
                    walker.Walk(breakExpression.Value, arg);
                    break;
                case IForeachExpressionSyntax foreachExpression:
                    walker.Walk(foreachExpression.TypeSyntax, arg);
                    walker.Walk(foreachExpression.InExpression, arg);
                    walker.Walk(foreachExpression.Block, arg);
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
                case ICallableNameSyntax _:
                case IUsingDirectiveSyntax _:
                case IParameterLifetimeBoundSyntax _:
                    // No Children
                    break;
            }
        }

        public static void WalkChildren(this ISyntaxWalker walker, ISyntax syntax)
        {
            walker.WalkChildren(syntax, default);
        }
    }
}
