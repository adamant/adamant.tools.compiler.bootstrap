using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Represents an extire package worth of syntax
    /// </summary>
    /// <remarks>Doesn't inherit from <see cref="Syntax"/> because it is never
    /// matched as part of syntax. It is always treated as the singular root.</remarks>
    public class PackageSyntax
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
