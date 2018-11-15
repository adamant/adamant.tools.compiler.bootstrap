using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class GenericsInvocationSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Callee { get; set; }
        [NotNull] public IOpenBracketTokenPlace OpenBracket { get; }
        [NotNull] public SeparatedListSyntax<ArgumentSyntax> ArgumentList { get; }
        [NotNull] public IEnumerable<ArgumentSyntax> Arguments => ArgumentList.Nodes();
        [NotNull] public ICloseBracketTokenPlace CloseBracket { get; }

        public GenericsInvocationSyntax(
            [NotNull] ExpressionSyntax callee,
            [NotNull] IOpenBracketTokenPlace openBracket,
            [NotNull] SeparatedListSyntax<ArgumentSyntax> argumentList,
            [NotNull] ICloseBracketTokenPlace closeBracket)
            : base(TextSpan.Covering(callee.Span, closeBracket.Span))
        {
            Callee = callee;
            OpenBracket = openBracket;
            ArgumentList = argumentList;
            CloseBracket = closeBracket;
        }

        public override string ToString()
        {
            return $"{Callee}[{Arguments}]";
        }
    }
}
