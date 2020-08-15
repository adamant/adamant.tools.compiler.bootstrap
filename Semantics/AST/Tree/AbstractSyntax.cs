using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal abstract class AbstractSyntax : IAbstractSyntax
    {
        public TextSpan Span { get; }

        protected AbstractSyntax(TextSpan span)
        {
            Span = span;
        }
    }
}
