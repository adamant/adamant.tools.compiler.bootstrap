using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class InvocationSyntax : ExpressionSyntax
    {
        public ExpressionSyntax Callee { get; set; }
        [CanBeNull]
        public OpenParenToken OpenParen { get; }
        [NotNull]
        public SeparatedListSyntax<ExpressionSyntax> Arguments { get; }
        [CanBeNull]
        public CloseParenToken CloseParen { get; }

        public InvocationSyntax(
            ExpressionSyntax callee,
            [CanBeNull] OpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ExpressionSyntax> arguments,
            [CanBeNull]CloseParenToken closeParen)
        {
            Requires.NotNull(nameof(arguments), arguments);
            Callee = callee;
            OpenParen = openParen;
            Arguments = arguments;
            CloseParen = closeParen;
        }
    }
}
