using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.CST.Walkers;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Validation
{
    /// <summary>
    /// Validates that all types are fulfilled. That is that everything as an
    /// assigned type, even if that type is Unknown.
    /// </summary>
    public class TypeFulfillmentValidator : SyntaxWalker
    {
        public void Walk(IEnumerable<IEntityDeclarationSyntax> entityDeclarations)
        {
            foreach (var declaration in entityDeclarations)
                WalkNonNull(declaration);
        }

        protected override void WalkNonNull(ISyntax syntax)
        {
            switch (syntax)
            {
                case IClassDeclarationSyntax classDeclaration:
                    classDeclaration.DeclaresDataType.Fulfilled();
                    // Don't recur into body, we will see those as separate members
                    return;
                case IConstructorDeclarationSyntax constructorDeclaration:
                    WalkChildren(constructorDeclaration);
                    constructorDeclaration.SelfParameterType.Assigned();
                    return;
                case IMethodDeclarationSyntax methodDeclaration:
                    WalkChildren(methodDeclaration);
                    methodDeclaration.ReturnDataType.Fulfilled();
                    return;
                case IFunctionDeclarationSyntax functionDeclaration:
                    WalkChildren(functionDeclaration);
                    functionDeclaration.ReturnDataType.Fulfilled();
                    return;
                case IAssociatedFunctionDeclarationSyntax associatedFunctionDeclaration:
                    WalkChildren(associatedFunctionDeclaration);
                    associatedFunctionDeclaration.ReturnDataType.Fulfilled();
                    return;
                case IParameterSyntax parameter:
                    WalkChildren(parameter);
                    parameter.DataType.Fulfilled();
                    return;
                case IFieldDeclarationSyntax fieldDeclaration:
                    WalkChildren(fieldDeclaration);
                    fieldDeclaration.DataType.Fulfilled();
                    return;
                case ITypeSyntax type:
                    WalkChildren(type);
                    type.NamedType.Assigned();
                    return;
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    WalkChildren(variableDeclaration);
                    variableDeclaration.DataType.Assigned();
                    return;
                case IForeachExpressionSyntax foreachExpression:
                    WalkChildren(foreachExpression);
                    foreachExpression.Type.Assigned();
                    foreachExpression.VariableType.Assigned();
                    return;
                case IExpressionSyntax expression:
                    WalkChildren(expression);
                    expression.Type.Assigned();
                    return;
            }

            WalkChildren(syntax);
        }
    }
}
