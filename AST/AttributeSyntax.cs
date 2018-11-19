using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class AttributeSyntax : Syntax
    {
        [NotNull] public NameSyntax Name { get; }
        [NotNull] public FixedList<ArgumentSyntax> Arguments { get; }

        public AttributeSyntax(
            [NotNull] NameSyntax name,
            [CanBeNull] FixedList<ArgumentSyntax> arguments)
        {
            Name = name;
            Arguments = arguments ?? FixedList<ArgumentSyntax>.Empty;
        }

        public override string ToString()
        {

            return $"{Name}({string.Join(", ", Arguments)})";
        }
    }
}
