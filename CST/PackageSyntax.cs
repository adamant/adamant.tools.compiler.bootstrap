using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// Represents an entire package worth of syntax
    /// </summary>
    /// <remarks>Doesn't inherit from <see cref="ISyntax"/> because it is never
    /// matched as part of syntax. It is always treated as the singular root.</remarks>
    public class PackageSyntax
    {
        public string Name { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public FixedList<ICompilationUnitSyntax> CompilationUnits { get; }

        public IEnumerable<Diagnostic> Diagnostics =>
            CompilationUnits.SelectMany(cu => cu.Diagnostics);

        public PackageSyntax(
            string name,
            FixedList<ICompilationUnitSyntax> compilationUnits)
        {
            Name = name;
            CompilationUnits = compilationUnits;
        }

        public IEnumerable<IDeclarationSyntax> GetDeclarations()
        {
            var declarations = new Queue<IDeclarationSyntax>();
            declarations.EnqueueRange(CompilationUnits.SelectMany(cu => cu.Declarations));
            while (declarations.TryDequeue(out var declaration))
            {
                yield return declaration;
                if (declaration is INamespaceDeclarationSyntax ns)
                    declarations.EnqueueRange(ns.Declarations);
            }
        }

        public override string ToString()
        {
            return $"package {Name}: {CompilationUnits.Count} Compilation Units";
        }
    }
}
