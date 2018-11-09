using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class NewObjectExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public INewKeywordTokenPlace NewKeyword { get; }
        [NotNull] public NameSyntax Constructor { get; }
        [NotNull] public IOpenParenTokenPlace OpenParen { get; }
        [NotNull] public SeparatedListSyntax<ArgumentSyntax> ArgumentList { get; }
        [NotNull] [ItemNotNull] public IEnumerable<ArgumentSyntax> Arguments => ArgumentList.Nodes();
        [NotNull] public ICloseParenTokenPlace CloseParen { get; }

        public NewObjectExpressionSyntax(
            [NotNull] INewKeywordTokenPlace newKeyword,
            [NotNull] NameSyntax constructor,
            [NotNull] IOpenParenTokenPlace openParen,
            [NotNull] SeparatedListSyntax<ArgumentSyntax> argumentList,
            [NotNull] ICloseParenTokenPlace closeParen)
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
