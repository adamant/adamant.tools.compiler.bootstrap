using System.Diagnostics;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.CST.Walkers
{
    public abstract class NewSyntaxWalker<T>
    {
        [DebuggerHidden]
        protected void Walk(ISyntax? syntax, T arg)
        {
            if (syntax is null) return;
            WalkNonNull(syntax, arg);
        }

        protected abstract void WalkNonNull(ISyntax syntax, T arg);

        [DebuggerHidden]
        protected void WalkChildren(ISyntax syntax, T arg)
        {
            foreach (var child in syntax.Children())
                WalkNonNull(child, arg);
        }

        [DebuggerHidden]
        protected void WalkChildrenInReverse(ISyntax syntax, T arg)
        {
            foreach (var child in syntax.Children().Reverse())
                WalkNonNull(child, arg);
        }
    }

    public abstract class NewSyntaxWalker
    {
        [DebuggerHidden]
        protected void Walk(ISyntax? syntax)
        {
            if (syntax is null) return;
            WalkNonNull(syntax);
        }

        protected abstract void WalkNonNull(ISyntax syntax);

        [DebuggerHidden]
        protected void WalkChildren(ISyntax syntax)
        {
            foreach (var child in syntax.Children())
                WalkNonNull(child);
        }

        [DebuggerHidden]
        protected void WalkChildrenInReverse(ISyntax syntax)
        {
            foreach (var child in syntax.Children().Reverse())
                WalkNonNull(child);
        }
    }
}
