using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class OrPatternSyntax : PatternSyntax
    {
        [NotNull] public PatternSyntax LeftAlternative { get; }
        [NotNull] public PipeToken Pipe { get; }
        [NotNull] public PatternSyntax RightAlternative { get; }

        public OrPatternSyntax(
            [NotNull] PatternSyntax leftAlternative,
            [NotNull] PipeToken pipe,
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
