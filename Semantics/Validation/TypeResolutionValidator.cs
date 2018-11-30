using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Validation
{
    public class TypeResolutionValidator : DeclarationVisitor<Void>
    {
        public static void Validate([NotNull, ItemNotNull] FixedList<MemberDeclarationSyntax> namespacedDeclarations)
        {
            var validator = new TypeResolutionValidator();
            foreach (var declaration in namespacedDeclarations)
                validator.VisitDeclaration(declaration);
        }

        public override void VisitDeclaration([CanBeNull] DeclarationSyntax declaration, Void args)
        {
            // Skip poisoned declarations
            if (declaration?.Poisoned ?? true) return;

            base.VisitDeclaration(declaration, args);
        }

        public override void VisitTypeDeclaration([CanBeNull] TypeDeclarationSyntax typeDeclaration, Void args)
        {
            base.VisitTypeDeclaration(typeDeclaration, args);
            typeDeclaration?.Type.Resolved();
        }

        public override void VisitFunctionDeclaration([CanBeNull] FunctionDeclarationSyntax functionDeclaration, Void args)
        {
            base.VisitFunctionDeclaration(functionDeclaration, args);
            functionDeclaration?.ReturnType.Resolved();
        }

        public override void VisitExpression(ExpressionSyntax expression, Void args)
        {
            base.VisitExpression(expression, args);
            expression?.Type.Resolved();
        }
    }
}
