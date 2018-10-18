using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class InvocationSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Callee { get; set; }
        [NotNull] public IOpenParenToken OpenParen { get; }
        [NotNull] public SeparatedListSyntax<ExpressionSyntax> Arguments { get; }
        [NotNull] public ICloseParenToken CloseParen { get; }

        public InvocationSyntax(
            [NotNull] ExpressionSyntax callee,
            [NotNull] IOpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ExpressionSyntax> arguments,
            [NotNull] ICloseParenToken closeParen)
        {
            Requires.NotNull(nameof(arguments), arguments);
            Callee = callee;
            OpenParen = openParen;
            Arguments = arguments;
            CloseParen = closeParen;
        }
    }
}
