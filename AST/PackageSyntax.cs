using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Represents an entire package worth of syntax
    /// </summary>
    /// <remarks>Doesn't inherit from <see cref="Syntax"/> because it is never
    /// matched as part of syntax. It is always treated as the singular root.</remarks>
    public class PackageSyntax
    {
        public string Name { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public FixedList<CompilationUnitSyntax> CompilationUnits { get; }

        public IEnumerable<Diagnostic> Diagnostics =>
            CompilationUnits.SelectMany(cu => cu.Diagnostics);

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
