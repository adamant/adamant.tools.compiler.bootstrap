using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.FST.Walkers
{
    public abstract class SyntaxWalker : ISyntaxWalker
    {
        [DebuggerHidden]
        public void Walk(ISyntax? syntax)
        {
            if (syntax is null) return;
            WalkNonNull(syntax);
        }

        [DebuggerHidden]
        void ISyntaxWalker<Void>.Walk(ISyntax? syntax, Void arg)
        {
            if (syntax is null) return;
            WalkNonNull(syntax);
        }

        protected abstract void WalkNonNull(ISyntax syntax);

        [DebuggerHidden]
        protected void WalkChildren(ISyntax syntax) =>
            this.WalkChildren(syntax, default);
        [DebuggerHidden]
        protected void WalkChildrenInReverse(ISyntax syntax) =>
            this.WalkChildrenInReverse(syntax, default);
    }

    public abstract class SyntaxWalker<T> : ISyntaxWalker<T>
    {
        [DebuggerHidden]
        public void Walk(ISyntax? syntax, T arg)
        {
            if (syntax is null) return;
            WalkNonNull(syntax, arg);
        }

        protected abstract void WalkNonNull(ISyntax syntax, T arg);

        [DebuggerHidden]
        protected void WalkChildren(ISyntax syntax, T arg) =>
            SyntaxWalkerExtensions.WalkChildren(this, syntax, arg);
        [DebuggerHidden]
        protected void WalkChildrenInReverse(ISyntax syntax, T arg) =>
            SyntaxWalkerExtensions.WalkChildrenInReverse(this, syntax, arg);
    }
}
