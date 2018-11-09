using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class GenericsInvocationSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Callee { get; set; }
        [NotNull] public IOpenBracketToken OpenBracket { get; }
        [NotNull] public SeparatedListSyntax<ArgumentSyntax> ArgumentList { get; }
        [NotNull] public IEnumerable<ArgumentSyntax> Arguments => ArgumentList.Nodes();
        [NotNull] public ICloseBracketToken CloseBracket { get; }

        public GenericsInvocationSyntax(
            [NotNull] ExpressionSyntax callee,
            [NotNull] IOpenBracketToken openBracket,
            [NotNull] SeparatedListSyntax<ArgumentSyntax> argumentList,
            [NotNull] ICloseBracketToken closeBracket)
            : base(TextSpan.Covering(callee.Span, closeBracket.Span))
        {
            Requires.NotNull(nameof(callee), callee);
            Requires.NotNull(nameof(openBracket), openBracket);
            Requires.NotNull(nameof(argumentList), argumentList);
            Requires.NotNull(nameof(closeBracket), closeBracket);
            Callee = callee;
            OpenBracket = openBracket;
            ArgumentList = argumentList;
            CloseBracket = closeBracket;
        }
    }
}
