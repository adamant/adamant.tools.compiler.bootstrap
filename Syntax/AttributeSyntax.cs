using System.Collections.Generic;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class AttributeSyntax : NonTerminal
    {
        [NotNull] public NameSyntax Name { get; }
        [NotNull] public SeparatedListSyntax<ArgumentSyntax> ArgumentList { get; }
        [NotNull] public IEnumerable<ArgumentSyntax> Arguments => ArgumentList.Nodes();

        public AttributeSyntax(
            [NotNull] NameSyntax name,
            [CanBeNull] SeparatedListSyntax<ArgumentSyntax> argumentList)
        {
            Name = name;
            ArgumentList = argumentList ?? SeparatedListSyntax<ArgumentSyntax>.Empty;
        }
    }
}
