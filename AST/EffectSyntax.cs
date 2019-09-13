using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(SimpleEffectSyntax),
        typeof(ThrowEffectSyntax))]
    public abstract class EffectSyntax : Syntax
    {
    }
}
