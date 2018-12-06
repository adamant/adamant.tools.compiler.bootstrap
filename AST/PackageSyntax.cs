using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public class PackageSyntax : Syntax
    {
        public string Name { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public FixedList<CompilationUnitSyntax> CompilationUnits { get; }

        public PackageSyntax(
            string name,
            FixedList<CompilationUnitSyntax> compilationUnits)
        {
            Name = name;
            CompilationUnits = compilationUnits;
        }

        public override string ToString()
        {
            return $"package {Name} {CompilationUnits.Count} Compilation Units";
        }
    }
}
