using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public abstract class SyntaxWalker : ISyntaxWalker
    {
        public void Walk(ISyntax? syntax)
        {
            if (syntax == null) return;
            WalkNonNull(syntax);
        }

        void ISyntaxWalker<Void>.Walk(ISyntax? syntax, Void arg)
        {
            if (syntax == null) return;
            WalkNonNull(syntax);
        }

        protected abstract void WalkNonNull(ISyntax syntax);
        protected void WalkChildren(ISyntax syntax) => this.WalkChildren(syntax, default);
    }

    public abstract class SyntaxWalker<T> : ISyntaxWalker<T>
    {
        public void Walk(ISyntax? syntax, T arg)
        {
            if (syntax == null) return;
            WalkNonNull(syntax, arg);
        }

        protected abstract void WalkNonNull(ISyntax syntax, T arg);
        protected void WalkChildren(ISyntax syntax, T arg) => SyntaxWalkerExtensions.WalkChildren(this, syntax, arg);
    }
}
