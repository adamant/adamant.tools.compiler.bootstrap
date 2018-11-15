using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class InvocationSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax Callee { get; set; }
        [NotNull] public IOpenParenTokenPlace OpenParen { get; }
        [NotNull] public SeparatedListSyntax<ArgumentSyntax> ArgumentList { get; }
        [NotNull] [ItemNotNull] public IEnumerable<ArgumentSyntax> Arguments => ArgumentList.Nodes();
        [NotNull] public ICloseParenTokenPlace CloseParen { get; }

        public InvocationSyntax(
            [NotNull] ExpressionSyntax callee,
            [NotNull] IOpenParenTokenPlace openParen,
            [NotNull] SeparatedListSyntax<ArgumentSyntax> argumentList,
            [NotNull] ICloseParenTokenPlace closeParen)
            : base(TextSpan.Covering(callee.Span, closeParen.Span))
        {
            Callee = callee;
            OpenParen = openParen;
            ArgumentList = argumentList;
            CloseParen = closeParen;
        }

        public override string ToString()
        {
            return $"{Callee}({Arguments})";
        }
    }
}
