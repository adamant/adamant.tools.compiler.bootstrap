using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public static class DeclarationVisitorExtensions
    {
        public static void VisitDeclarations([NotNull] this DeclarationVisitor<Void> visitor, [NotNull] IEnumerable<DeclarationSyntax> declaration)
        {
            visitor.VisitDeclarations(declaration, default);
        }

        public static void VisitDeclaration([NotNull] this DeclarationVisitor<Void> visitor, [CanBeNull] DeclarationSyntax declaration)
        {
            visitor.VisitDeclaration(declaration, default);
        }
    }
}
