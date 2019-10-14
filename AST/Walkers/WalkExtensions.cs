using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public static class WalkExtensions
    {
        public static void Walk(this ISyntaxWalker walker, ISyntax? syntax)
        {
            var traversal = new SyntaxTraversal(walker);
            traversal.Walk(syntax);
        }

        public static FixedList<IVariableDeclarationStatementSyntax> GetAllVariableDeclarations(
            this IBodySyntax body)
        {
            var collector = new VariableDeclarationsCollector();
            collector.Walk(body);
            return collector.Declarations;
        }
    }
}
