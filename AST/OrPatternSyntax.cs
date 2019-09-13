namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class OrPatternSyntax : PatternSyntax
    {
        public PatternSyntax LeftAlternative { get; }
        public PatternSyntax RightAlternative { get; }

        public OrPatternSyntax(
            PatternSyntax leftAlternative,
            PatternSyntax rightAlternative)
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
