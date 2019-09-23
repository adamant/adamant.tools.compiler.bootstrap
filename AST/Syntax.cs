using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(DeclarationSyntax),
        typeof(StatementSyntax),
        typeof(ParameterSyntax),
        typeof(UsingDirectiveSyntax),
        typeof(ModifierSyntax),
        typeof(CompilationUnitSyntax),
        typeof(ArgumentSyntax),
        typeof(TypeSyntax),
        typeof(ExpressionSyntax))]
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public abstract class Syntax
    {
        public TextSpan Span { get; }

        private protected Syntax(TextSpan span)
        {
            Span = span;
        }

        // This exists primarily for debugging use
        public abstract override string ToString();
    }
}
