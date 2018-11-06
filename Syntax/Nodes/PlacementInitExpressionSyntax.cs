using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class PlacementInitExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IInitKeywordToken InitKeyword { get; }
        [NotNull] public IOpenParenToken OpenParen { get; }
        [NotNull] public ExpressionSyntax PlaceExpression { get; }
        [NotNull] public ICloseParenToken CloseParen { get; }
        [NotNull] public NameSyntax Initializer { get; }
        [NotNull] public IOpenParenToken ArgumentsOpenParen { get; }
        [NotNull] public SeparatedListSyntax<ArgumentSyntax> ArgumentList { get; }
        [NotNull] [ItemNotNull] public IEnumerable<ArgumentSyntax> Arguments => ArgumentList.Nodes();
        [NotNull] public ICloseParenToken ArgumentsCloseParen { get; }

        public PlacementInitExpressionSyntax(
            [NotNull] IInitKeywordToken initKeyword,
            [NotNull] IOpenParenToken openParen,
            [NotNull] ExpressionSyntax placeExpression,
            [NotNull] ICloseParenToken closeParen,
            [NotNull] NameSyntax initializer,
            [NotNull] IOpenParenToken argumentsOpenParen,
            [NotNull] SeparatedListSyntax<ArgumentSyntax> argumentList,
            [NotNull] ICloseParenToken argumentsCloseParen)
            : base(TextSpan.Covering(initKeyword.Span, argumentsCloseParen.Span))
        {
            Requires.NotNull(nameof(initKeyword), initKeyword);
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(placeExpression), placeExpression);
            Requires.NotNull(nameof(closeParen), closeParen);
            Requires.NotNull(nameof(initializer), initializer);
            Requires.NotNull(nameof(argumentsOpenParen), argumentsOpenParen);
            Requires.NotNull(nameof(argumentList), argumentList);
            Requires.NotNull(nameof(argumentsCloseParen), argumentsCloseParen);
            InitKeyword = initKeyword;
            OpenParen = openParen;
            PlaceExpression = placeExpression;
            CloseParen = closeParen;
            Initializer = initializer;
            ArgumentsOpenParen = argumentsOpenParen;
            ArgumentList = argumentList;
            ArgumentsCloseParen = argumentsCloseParen;
        }
    }
}
