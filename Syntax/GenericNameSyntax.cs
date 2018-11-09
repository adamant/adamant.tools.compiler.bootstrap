using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class GenericNameSyntax : SimpleNameSyntax
    {
        [NotNull] public IOpenBracketToken OpenBracket { get; }
        [NotNull] public SeparatedListSyntax<ArgumentSyntax> ArgumentList { get; }
        [NotNull] public IEnumerable<ArgumentSyntax> Arguments => ArgumentList.Nodes();
        [NotNull] public ICloseBracketToken CloseBracket { get; }

        public GenericNameSyntax(
            [NotNull] IIdentifierToken name,
            [NotNull] IOpenBracketToken openBracket,
            [NotNull] SeparatedListSyntax<ArgumentSyntax> argumentList,
            [NotNull] ICloseBracketToken closeBracket)
            : base(name, TextSpan.Covering(name.Span, closeBracket.Span))
        {
            Requires.NotNull(nameof(openBracket), openBracket);
            Requires.NotNull(nameof(argumentList), argumentList);
            Requires.NotNull(nameof(closeBracket), closeBracket);
            OpenBracket = openBracket;
            ArgumentList = argumentList;
            CloseBracket = closeBracket;
        }
    }
}
