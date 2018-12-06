using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Validation
{
    public class TypeResolutionValidator : DeclarationVisitor<Void>
    {
        public static void Validate(FixedList<MemberDeclarationSyntax> namespacedDeclarations)
        {
            var validator = new TypeResolutionValidator();
            foreach (var declaration in namespacedDeclarations)
                validator.VisitDeclaration(declaration);
        }

        public override void VisitDeclaration(DeclarationSyntax declaration, Void args)
        {
            // Skip poisoned declarations
            if (declaration?.Poisoned ?? true) return;

            base.VisitDeclaration(declaration, args);
        }

        public override void VisitTypeDeclaration(TypeDeclarationSyntax typeDeclaration, Void args)
        {
            base.VisitTypeDeclaration(typeDeclaration, args);
            typeDeclaration?.Type.Resolved();
        }

        public override void VisitFunctionDeclaration(FunctionDeclarationSyntax functionDeclaration, Void args)
        {
            base.VisitFunctionDeclaration(functionDeclaration, args);
            functionDeclaration?.ReturnType.Resolved();
        }

        public override void VisitExpression(ExpressionSyntax expression, Void args)
        {
            base.VisitExpression(expression, args);
            expression?.Type.AssertResolved();
        }
    }
}
