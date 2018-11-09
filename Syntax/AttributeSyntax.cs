using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class AttributeSyntax : SyntaxNode
    {
        [NotNull] public HashToken Hash { get; }
        [NotNull] public NameSyntax Name { get; }
        [CanBeNull] public IOpenParenToken OpenParen { get; }
        [NotNull] public SeparatedListSyntax<ArgumentSyntax> ArgumentList { get; }
        [NotNull] public IEnumerable<ArgumentSyntax> Arguments => ArgumentList.Nodes();
        [CanBeNull] public ICloseParenToken CloseParen { get; }

        public AttributeSyntax(
            [NotNull] HashToken hash,
            [NotNull] NameSyntax name,
            [CanBeNull] IOpenParenToken openParen,
            [CanBeNull] SeparatedListSyntax<ArgumentSyntax> argumentList,
            [CanBeNull] ICloseParenToken closeParen)
        {
            Hash = hash;
            Name = name;
            OpenParen = openParen;
            ArgumentList = argumentList ?? SeparatedListSyntax<ArgumentSyntax>.Empty;
            CloseParen = closeParen;
        }

        [ItemNotNull]
        public IEnumerable<IToken> Tokens()
        {
            yield return Hash;
            // TODO tokens for Name
            if (OpenParen != null) yield return OpenParen;
            // TODO tokens for ArgumentList
            if (CloseParen != null) yield return CloseParen;
        }
    }
}
