using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions
{
    public class NewObjectExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public INewKeywordToken NewKeyword { get; }
        [NotNull] public TypeSyntax Type { get; }
        [NotNull] public IOpenParenToken OpenParen { get; }
        [NotNull] public SeparatedListSyntax<ExpressionSyntax> ArgumentList { get; }
        [NotNull] [ItemNotNull] public IEnumerable<ExpressionSyntax> Arguments => ArgumentList.Nodes();
        [NotNull] public ICloseParenToken CloseParen { get; }

        public NewObjectExpressionSyntax(
            [NotNull] INewKeywordToken newKeyword,
            [NotNull] TypeSyntax type,
            [NotNull] IOpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ExpressionSyntax> argumentList,
            [NotNull] ICloseParenToken closeParen)
            : base(TextSpan.Covering(newKeyword.Span, closeParen.Span))
        {
            Requires.NotNull(nameof(argumentList), argumentList);
            NewKeyword = newKeyword;
            Type = type;
            OpenParen = openParen;
            ArgumentList = argumentList;
            CloseParen = closeParen;
        }
    }
}
