using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class PackageSyntax : NonTerminal
    {
        [NotNull] public string Name { get; }
        [NotNull] public SyntaxList<CompilationUnitSyntax> CompilationUnits { get; }

        public PackageSyntax(
            [NotNull] string name,
            [NotNull] SyntaxList<CompilationUnitSyntax> compilationUnits)
        {
            Name = name;
            CompilationUnits = compilationUnits;
        }
    }
}
