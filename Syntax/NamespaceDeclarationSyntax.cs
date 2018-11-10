using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public abstract class NamespaceDeclarationSyntax : DeclarationSyntax
    {
        [CanBeNull] public NameSyntax Name { get; }
        [NotNull] public FixedList<UsingDirectiveSyntax> UsingDirectives { get; }
        [NotNull] public FixedList<DeclarationSyntax> Declarations { get; }

        protected NamespaceDeclarationSyntax(
            [CanBeNull] NameSyntax name,
            [NotNull] FixedList<UsingDirectiveSyntax> usingDirectives,
            [NotNull] FixedList<DeclarationSyntax> declarations)
        {
            Name = name;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
        }
    }
}
