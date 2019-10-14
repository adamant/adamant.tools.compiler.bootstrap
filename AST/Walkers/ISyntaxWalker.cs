namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public interface ISyntaxWalker
    {
        /// <returns>Whether to walk this node's children</returns>
        bool Enter(ISyntax syntax, ISyntaxTraversal traversal);

        void Exit(ISyntax syntax, ISyntaxTraversal traversal);
    }
}
