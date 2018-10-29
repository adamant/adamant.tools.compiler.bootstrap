using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class GenericsInvocationSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Callee { get; set; }
        [NotNull] public IOpenBracketToken OpenBracket { get; }
        [NotNull] public SeparatedListSyntax<ArgumentSyntax> Arguments { get; }
        [NotNull] public ICloseBracketToken CloseBracket { get; }

        public GenericsInvocationSyntax(
            [NotNull] ExpressionSyntax callee,
            [NotNull] IOpenBracketToken openBracket,
            [NotNull] SeparatedListSyntax<ArgumentSyntax> arguments,
            [NotNull] ICloseBracketToken closeBracket)
            : base(TextSpan.Covering(callee.Span, closeBracket.Span))
        {
            Requires.NotNull(nameof(callee), callee);
            Requires.NotNull(nameof(openBracket), openBracket);
            Requires.NotNull(nameof(arguments), arguments);
            Requires.NotNull(nameof(closeBracket), closeBracket);
            Callee = callee;
            OpenBracket = openBracket;
            Arguments = arguments;
            CloseBracket = closeBracket;
        }
    }
}
