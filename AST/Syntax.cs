using System.Diagnostics;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(DeclarationSyntax),
        typeof(StatementSyntax),
        typeof(PatternSyntax),
        typeof(ParameterSyntax),
        typeof(UsingDirectiveSyntax),
        typeof(ThrowEffectEntrySyntax),
        typeof(PackageSyntax),
        typeof(EffectSyntax),
        typeof(ModifierSyntax),
        typeof(MatchArmSyntax),
        typeof(GenericParameterSyntax),
        typeof(GenericConstraintSyntax),
        typeof(AttributeSyntax),
        typeof(EnumVariantSyntax),
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
