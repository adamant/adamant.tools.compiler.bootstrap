using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class AttributeSyntax : NonTerminal
    {
        [NotNull] public IHashToken Hash { get; }
        [NotNull] public NameSyntax Name { get; }
        [CanBeNull] public IOpenParenTokenPlace OpenParen { get; }
        [NotNull] public SeparatedListSyntax<ArgumentSyntax> ArgumentList { get; }
        [NotNull] public IEnumerable<ArgumentSyntax> Arguments => ArgumentList.Nodes();
        [CanBeNull] public ICloseParenTokenPlace CloseParen { get; }

        public AttributeSyntax(
            [NotNull] IHashToken hash,
            [NotNull] NameSyntax name,
            [CanBeNull] IOpenParenTokenPlace openParen,
            [CanBeNull] SeparatedListSyntax<ArgumentSyntax> argumentList,
            [CanBeNull] ICloseParenTokenPlace closeParen)
        {
            Hash = hash;
            Name = name;
            OpenParen = openParen;
            ArgumentList = argumentList ?? SeparatedListSyntax<ArgumentSyntax>.Empty;
            CloseParen = closeParen;
        }

        [ItemNotNull]
        public IEnumerable<ITokenPlace> Tokens()
        {
            yield return Hash;
            // TODO tokens for Name
            if (OpenParen != null) yield return OpenParen;
            // TODO tokens for ArgumentList
            if (CloseParen != null) yield return CloseParen;
        }
    }
}
