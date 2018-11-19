using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
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

        [NotNull]
        public override string ToString()
        {
            if (IsParams) return $"params {Value}";
            return Value?.ToString() ?? "";
        }
    }
}
