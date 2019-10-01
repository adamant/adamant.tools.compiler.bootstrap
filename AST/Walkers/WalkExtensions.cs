namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public static class WalkExtensions
    {
        public static void Walk(this ITypeWalker walker, ITypeSyntax? type)
        {
            var treeWalker = new TreeWalker(null, null, walker);
            treeWalker.Walk(type);
        }
    }
}
