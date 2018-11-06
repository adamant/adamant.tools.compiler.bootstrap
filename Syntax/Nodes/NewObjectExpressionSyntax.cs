using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes
{
    public class NewObjectExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public INewKeywordToken NewKeyword { get; }
        [NotNull] public NameSyntax Constructor { get; }
        [NotNull] public IOpenParenToken OpenParen { get; }
        [NotNull] public SeparatedListSyntax<ArgumentSyntax> ArgumentList { get; }
        [NotNull] [ItemNotNull] public IEnumerable<ArgumentSyntax> Arguments => ArgumentList.Nodes();
        [NotNull] public ICloseParenToken CloseParen { get; }

        public NewObjectExpressionSyntax(
            [NotNull] INewKeywordToken newKeyword,
            [NotNull] NameSyntax constructor,
            [NotNull] IOpenParenToken openParen,
            [NotNull] SeparatedListSyntax<ArgumentSyntax> argumentList,
            [NotNull] ICloseParenToken closeParen)
            : base(TextSpan.Covering(newKeyword.Span, closeParen.Span))
        {
            Requires.NotNull(nameof(newKeyword), newKeyword);
            Requires.NotNull(nameof(constructor), constructor);
            Requires.NotNull(nameof(openParen), openParen);
            Requires.NotNull(nameof(argumentList), argumentList);
            Requires.NotNull(nameof(closeParen), closeParen);
            NewKeyword = newKeyword;
            Constructor = constructor;
            OpenParen = openParen;
            ArgumentList = argumentList;
            CloseParen = closeParen;
        }
    }
}
