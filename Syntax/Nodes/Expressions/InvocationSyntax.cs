using Adamant.Tools.Compiler.Bootstrap.Core;
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
            : base(TextSpan.Covering(callee.Span, closeParen.Span))
        {
            Requires.NotNull(nameof(callee), callee);
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(arguments), arguments);
            Requires.NotNull(nameof(closeParen), closeParen);
            Callee = callee;
            OpenParen = openParen;
            Arguments = arguments;
            CloseParen = closeParen;
        }
    }
}
