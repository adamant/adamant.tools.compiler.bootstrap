using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class InvocationSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Callee { get; set; }
        [NotNull, ItemNotNull] public FixedList<ArgumentSyntax> Arguments { get; }

        public InvocationSyntax(
            TextSpan span,
            [NotNull] ExpressionSyntax callee,
            [NotNull, ItemNotNull] FixedList<ArgumentSyntax> arguments)
            : base(span)
        {
            Callee = callee;
            Arguments = arguments;
        }

        public override string ToString()
        {
            return $"{Callee}({Arguments})";
        }
    }
}
