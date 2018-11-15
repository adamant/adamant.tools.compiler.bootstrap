using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class ArgumentSyntax : NonTerminal
    {
        public bool IsParams { get; }
        [CanBeNull] public ExpressionSyntax Value { get; }

        public ArgumentSyntax(
            bool isParams,
            [CanBeNull] ExpressionSyntax value)
        {
            IsParams = isParams;
            Value = value;
        }
    }
}
