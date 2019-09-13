namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class MatchArmSyntax : Syntax
    {
        public PatternSyntax Pattern { get; }
        public ExpressionBlockSyntax Expression { get; }

        public MatchArmSyntax(
            PatternSyntax pattern,
            ExpressionBlockSyntax expression)
        {
            Pattern = pattern;
            Expression = expression;
        }

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
