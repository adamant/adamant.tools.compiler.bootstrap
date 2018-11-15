using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    // TODO maybe this class should be removed in favor of just using NameSyntax?
    public class UsingDirectiveSyntax : NonTerminal
    {
        [NotNull] public NameSyntax Name { get; }

        public UsingDirectiveSyntax([NotNull] NameSyntax name)
        {
            Name = name;
        }
    }
}
