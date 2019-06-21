using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    // An expression is also a statement
    public abstract class ExpressionSyntax : StatementSyntax
    {
        public TextSpan Span { get; }
        public ValueSemantics? ValueSemantics { get; set; } = null;

        /// <summary>
        /// If an expression has been poisoned, then it is errored in some way
        /// and we won't report errors against it in the future. We may also
        /// skip it for some processing.
        /// </summary>
        public bool Poisoned { get; private set; }

        protected ExpressionSyntax(TextSpan span)
        {
            Span = span;
        }

        public void Poison()
        {
            Poisoned = true;
        }

        // Useful for debugging
        public abstract override string ToString();
    }
}
