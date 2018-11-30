using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Validation
{
    public class TypeResolutionValidator : DeclarationVisitor<Void>
    {
        public static void Validate([NotNull] FixedList<INamespacedDeclarationSyntax> namespacedDeclarations)
        {
            var validator = new TypeResolutionValidator();
            foreach (var declaration in namespacedDeclarations.Select(d => d.AsDeclarationSyntax)
                .Where(d => !d.Poisoned))
                validator.VisitDeclaration(declaration);
        }

        public override void VisitTypeDeclaration([CanBeNull] TypeDeclarationSyntax typeDeclaration, Void args)
        {
            base.VisitTypeDeclaration(typeDeclaration, args);
            typeDeclaration?.Type.Resolved();
        }

        public override void VisitFunctionDeclaration([CanBeNull] FunctionDeclarationSyntax function, Void args)
        {
            base.VisitFunctionDeclaration(function, args);
            function?.ReturnType.Resolved();
        }

        public override void VisitExpression(ExpressionSyntax expression, Void args)
        {
            base.VisitExpression(expression, args);
            expression?.Type.Resolved();
        }
    }
}
