using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class OrPatternSyntax : PatternSyntax
    {
        [NotNull] public PatternSyntax LeftAlternative { get; }
        [NotNull] public IPipeToken Pipe { get; }
        [NotNull] public PatternSyntax RightAlternative { get; }

        public OrPatternSyntax(
            [NotNull] PatternSyntax leftAlternative,
            [NotNull] IPipeToken pipe,
            [NotNull] PatternSyntax rightAlternative)
        {
            Requires.NotNull(nameof(leftAlternative), leftAlternative);
            Requires.NotNull(nameof(pipe), pipe);
            Requires.NotNull(nameof(rightAlternative), rightAlternative);
            LeftAlternative = leftAlternative;
            Pipe = pipe;
            RightAlternative = rightAlternative;
        }
    }
}