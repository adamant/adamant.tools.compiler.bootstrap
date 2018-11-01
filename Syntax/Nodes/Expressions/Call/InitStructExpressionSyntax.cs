using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions.Call
{
    public class InitStructExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public IInitKeywordToken InitKeyword { get; }
        [NotNull] public TypeSyntax Constructor { get; }
        [NotNull] public IOpenParenToken OpenParen { get; }
        [NotNull] public SeparatedListSyntax<ArgumentSyntax> ArgumentList { get; }
        [NotNull] [ItemNotNull] public IEnumerable<ArgumentSyntax> Arguments => ArgumentList.Nodes();
        [NotNull] public ICloseParenToken CloseParen { get; }

        public InitStructExpressionSyntax(
            [NotNull] IInitKeywordToken initKeyword,
            [NotNull] TypeSyntax constructor,
            [NotNull] IOpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ArgumentSyntax> argumentList,
            [NotNull] ICloseParenToken closeParen)
            : base(TextSpan.Covering(initKeyword.Span, closeParen.Span))
        {
            Requires.NotNull(nameof(initKeyword), initKeyword);
            Requires.NotNull(nameof(constructor), constructor);
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(argumentList), argumentList);
            Requires.NotNull(nameof(closeParen), closeParen);
            InitKeyword = initKeyword;
            Constructor = constructor;
            OpenParen = openParen;
            ArgumentList = argumentList;
            CloseParen = closeParen;
        }
    }
}
