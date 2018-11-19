using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class GenericsInvocationSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Callee { get; set; }
        [NotNull] public FixedList<ArgumentSyntax> Arguments { get; }

        public GenericsInvocationSyntax(
            TextSpan span,
            [NotNull] ExpressionSyntax callee,
            [NotNull] FixedList<ArgumentSyntax> arguments)
            : base(span)
        {
            Callee = callee;
            Arguments = arguments;
        }

        public override string ToString()
        {
            return $"{Callee}[{Arguments}]";
        }
    }
}
