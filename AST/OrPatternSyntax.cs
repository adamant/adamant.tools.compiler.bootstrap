using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class OrPatternSyntax : PatternSyntax
    {
        [NotNull] public PatternSyntax LeftAlternative { get; }
        [NotNull] public PatternSyntax RightAlternative { get; }

        public OrPatternSyntax(
            [NotNull] PatternSyntax leftAlternative,
            [NotNull] PatternSyntax rightAlternative)
        {
            LeftAlternative = leftAlternative;
            RightAlternative = rightAlternative;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
