using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Types;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    // An expression is also a statement
    public abstract class ExpressionSyntax : StatementSyntax
    {
        public TextSpan Span { get; }
        [NotNull] public TypePromise Type { get; } = new TypePromise();

        protected ExpressionSyntax(TextSpan span)
        {
            Span = span;
        }

        // Useful for debugging
        public abstract override string ToString();
    }
}
