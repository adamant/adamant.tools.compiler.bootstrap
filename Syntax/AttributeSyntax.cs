using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class AttributeSyntax : NonTerminal
    {
        [NotNull] public NameSyntax Name { get; }
        [NotNull] public FixedList<ArgumentSyntax> Arguments { get; }

        public AttributeSyntax(
            [NotNull] NameSyntax name,
            [CanBeNull] FixedList<ArgumentSyntax> arguments)
        {
            Name = name;
            Arguments = arguments;
        }
    }
}
