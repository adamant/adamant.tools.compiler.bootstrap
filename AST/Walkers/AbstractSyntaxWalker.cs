using System.Diagnostics;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.AST.Walkers
{
    public abstract class AbstractSyntaxWalker<T>
    {
        [DebuggerHidden]
        protected void Walk(IAbstractSyntax? syntax, T arg)
        {
            if (syntax is null) return;
            WalkNonNull(syntax, arg);
        }

        protected abstract void WalkNonNull(IAbstractSyntax syntax, T arg);

        [DebuggerHidden]
        protected void WalkChildren(IAbstractSyntax syntax, T arg)
        {
            foreach (var child in syntax.Children())
                WalkNonNull(child, arg);
        }

        [DebuggerHidden]
        protected void WalkChildrenInReverse(IAbstractSyntax syntax, T arg)
        {
            foreach (var child in syntax.Children().Reverse())
                WalkNonNull(child, arg);
        }
    }

    public abstract class AbstractSyntaxWalker
    {
        [DebuggerHidden]
        protected void Walk(IAbstractSyntax? syntax)
        {
            if (syntax is null) return;
            WalkNonNull(syntax);
        }

        protected abstract void WalkNonNull(IAbstractSyntax syntax);

        [DebuggerHidden]
        protected void WalkChildren(IAbstractSyntax syntax)
        {
            foreach (var child in syntax.Children())
                WalkNonNull(child);
        }

        [DebuggerHidden]
        protected void WalkChildrenInReverse(IAbstractSyntax syntax)
        {
            foreach (var child in syntax.Children().Reverse())
                WalkNonNull(child);
        }
    }
}
