namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public abstract class SyntaxWalker : ISyntaxWalker
    {
        public void Walk(ISyntax? syntax)
        {
            if (syntax == null) return;
            WalkNonNull(syntax);
        }

        protected abstract void WalkNonNull(ISyntax syntax);
        protected void WalkChildren(ISyntax syntax) => SyntaxWalkerExtensions.WalkChildren(this, syntax);
    }
}
