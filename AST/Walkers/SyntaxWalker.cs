namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public abstract class SyntaxWalker : ISyntaxWalker
    {
        public abstract bool Enter(ISyntax syntax, ISyntaxTraversal traversal);

        public virtual void Exit(ISyntax syntax, ISyntaxTraversal traversal)
        {
        }
    }
}
