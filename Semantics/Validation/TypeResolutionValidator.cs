using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.AST.Visitors;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Validation
{
    public class TypeResolutionValidator : DeclarationVisitor<Void, Void>
    {
        public static void Validate([NotNull] FixedList<INamespacedDeclarationSyntax> namespacedDeclarations)
        {
            var validator = new TypeResolutionValidator();
            foreach (var declaration in namespacedDeclarations.Select(d => d.AsDeclarationSyntax)
                .Where(d => !d.Poisoned))
                validator.VisitDeclaration(declaration, default);
        }

        public override Void VisitExpression(ExpressionSyntax expression, Void args)
        {
            base.VisitExpression(expression, args);
            expression?.Type.Resolved();
            return default;
        }


    }
}
