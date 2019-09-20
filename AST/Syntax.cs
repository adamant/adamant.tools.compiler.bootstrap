using System.Diagnostics;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(DeclarationSyntax),
        typeof(StatementSyntax),
        typeof(ParameterSyntax),
        typeof(UsingDirectiveSyntax),
        typeof(PackageSyntax),
        typeof(ModifierSyntax),
        typeof(CompilationUnitSyntax),
        typeof(ArgumentSyntax))]
    [DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
    public abstract class Syntax
    {
        private protected Syntax() { }

        // This exists primarily for debugging use
        public abstract override string ToString();
    }
}
