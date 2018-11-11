using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class PackageSyntax : NonTerminal
    {
        [NotNull] public string Name { get; }
        [NotNull] public FixedList<CompilationUnitSyntax> CompilationUnits { get; }

        public PackageSyntax(
            [NotNull] string name,
            [NotNull] FixedList<CompilationUnitSyntax> compilationUnits)
        {
            Name = name;
            CompilationUnits = compilationUnits;
        }
    }
}
