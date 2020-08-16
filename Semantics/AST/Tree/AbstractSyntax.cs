using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    internal abstract class AbstractSyntax : IAbstractSyntax
    {
        public TextSpan Span { get; }

        protected AbstractSyntax(TextSpan span)
        {
            Span = span;
        }

        public abstract override string ToString();
    }
}
