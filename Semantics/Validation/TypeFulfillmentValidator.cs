using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Void = Adamant.Tools.Compiler.Bootstrap.Framework.Void;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Validation
{
    /// <summary>
    /// Validates that all types are fulfilled. That is that everything as an
    /// assigned type, even if that type is Unknown.
    /// </summary>
    public class TypeFulfillmentValidator : DeclarationVisitor<Void>
    {
        public static void Validate(IEnumerable<IEntityDeclarationSyntax> entityDeclarations)
        {
            var validator = new TypeFulfillmentValidator();
            foreach (var declaration in entityDeclarations)
                validator.VisitDeclaration(declaration);
        }

        public override void VisitClassDeclaration(IClassDeclarationSyntax classDeclaration, Void args)
        {
            classDeclaration?.DeclaresType.Fulfilled();
            // Don't recur into body, we will see those as separate members
        }

        public override void VisitMethodDeclaration(IMethodDeclarationSyntax methodDeclaration, Void args)
        {
            base.VisitMethodDeclaration(methodDeclaration, args);
            methodDeclaration?.ReturnType.Fulfilled();
        }

        public override void VisitVariableDeclarationStatement(
            IVariableDeclarationStatementSyntax variableDeclaration,
            Void args)
        {
            base.VisitVariableDeclarationStatement(variableDeclaration, args);
            if (variableDeclaration.Type == null)
                throw new Exception("Variable declaration doesn't have type");
        }

        public override void VisitExpression(IExpressionSyntax expression, Void args)
        {
            base.VisitExpression(expression, args);
            if (expression != null && expression.Type == null)
                throw new Exception("Expression doesn't have type");
        }
    }
}
