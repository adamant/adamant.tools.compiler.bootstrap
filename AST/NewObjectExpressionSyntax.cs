using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class NewObjectExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Constructor { get; }
        public FixedList<ArgumentSyntax> Arguments { get; }

        public NewObjectExpressionSyntax(
            TextSpan span,
            ExpressionSyntax constructor,
            FixedList<ArgumentSyntax> arguments)
            : base(span)
        {
            Constructor = constructor;
            Arguments = arguments;
        }

        public override string ToString()
        {
            return $"new {Constructor}({string.Join(", ", Arguments)})";
        }
    }
}
