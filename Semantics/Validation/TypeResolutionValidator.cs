using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Validation
{
    public class TypeResolutionValidator : DeclarationVisitor<Void>
    {
        public static void Validate(IEnumerable<MemberDeclarationSyntax> allMemberDeclarations)
        {
            var validator = new TypeResolutionValidator();
            foreach (var declaration in allMemberDeclarations)
                validator.VisitDeclaration(declaration);
        }

        public override void VisitDeclaration(DeclarationSyntax declaration, Void args)
        {
            // Skip poisoned declarations
            if (declaration?.HasErrors ?? true) return;

            base.VisitDeclaration(declaration, args);
        }

        public override void VisitTypeDeclaration(TypeDeclarationSyntax typeDeclaration, Void args)
        {
            base.VisitTypeDeclaration(typeDeclaration, args);
            typeDeclaration?.Type.Known();
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax classDeclaration, Void args)
        {
            // Don't recur into body, we will see those as separate members
        }

        public override void VisitFunctionDeclaration(FunctionDeclarationSyntax functionDeclaration, Void args)
        {
            base.VisitFunctionDeclaration(functionDeclaration, args);
            functionDeclaration?.ReturnType.Known();
            functionDeclaration?.Type.Known();
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax constructorDeclaration, Void args)
        {
            base.VisitConstructorDeclaration(constructorDeclaration, args);
            constructorDeclaration?.ReturnType.Known();
            constructorDeclaration?.Type.Known();
        }

        public override void VisitVariableDeclarationStatement(
            VariableDeclarationStatementSyntax variableDeclaration,
            Void args)
        {
            base.VisitVariableDeclarationStatement(variableDeclaration, args);
            variableDeclaration.Type.AssertKnown();
        }

        public override void VisitExpression(ExpressionSyntax expression, Void args)
        {
            base.VisitExpression(expression, args);
            expression?.Type.AssertKnown();
        }
    }
}
