using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Visitors
{
    public static class DeclarationVisitorExtensions
    {
        public static void VisitDeclaration(this DeclarationVisitor<Void> visitor,
            IDeclarationSyntax? declaration)
        {
            visitor.VisitDeclaration(declaration, default);
        }
    }
}
