using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class InvocationSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Callee { get; set; }
        public FixedList<ArgumentSyntax> Arguments { get; }

        public InvocationSyntax(
            TextSpan span,
            ExpressionSyntax callee,
            FixedList<ArgumentSyntax> arguments)
            : base(span)
        {
            Callee = callee;
            Arguments = arguments;
        }

        public override string ToString()
        {
            return $"{Callee}({string.Join(", ", Arguments)})";
        }
    }
}
