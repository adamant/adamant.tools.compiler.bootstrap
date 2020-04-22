using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Walkers;

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
                    classDeclaration.DeclaresType.Fulfilled();
                    // Don't recur into body, we will see those as separate members
                    return;
                case IConstructorDeclarationSyntax constructorDeclaration:
                    WalkChildren(constructorDeclaration);
                    if (constructorDeclaration.SelfParameterType is null)
                        throw new Exception("Constructor self parameter doesn't have type");
                    return;
                case IMethodDeclarationSyntax methodDeclaration:
                    WalkChildren(methodDeclaration);
                    methodDeclaration.ReturnType.Fulfilled();
                    return;
                case IFunctionDeclarationSyntax functionDeclaration:
                    WalkChildren(functionDeclaration);
                    functionDeclaration.ReturnType.Fulfilled();
                    return;
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    WalkChildren(variableDeclaration);
                    if (variableDeclaration.Type is null)
                        throw new Exception("Variable declaration doesn't have type");
                    return;
                case IExpressionSyntax expression:
                    WalkChildren(expression);
                    if (expression.Type is null)
                        throw new Exception("Expression doesn't have type");
                    return;
            }

            WalkChildren(syntax);
        }
    }
}
