using Adamant.Tools.Compiler.Bootstrap.Core;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class ArgumentSyntax : Syntax
    {
        public ExpressionSyntax Value;

        public ArgumentSyntax(
            TextSpan span,
            ExpressionSyntax value)
            : base(span)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "";
        }
    }
}
