using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(OrPatternSyntax),
        typeof(EnumValuePatternSyntax),
        typeof(AnyPatternSyntax))]
    public abstract class PatternSyntax : Syntax
    {
    }
}
