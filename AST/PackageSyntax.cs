using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class PackageSyntax : Syntax
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

        public override string ToString()
        {
            throw new System.NotImplementedException();
        }
    }
}
