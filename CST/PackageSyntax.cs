using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// Represents an entire package worth of syntax
    /// </summary>
    /// <remarks>Doesn't inherit from <see cref="ISyntax"/> because it is never
    /// matched as part of syntax. It is always treated as the singular root.</remarks>
    public class PackageSyntax
    {
        public PackageSymbol Symbol { get; }
        public SymbolTreeBuilder SymbolTreeBuilder { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public FixedList<ICompilationUnitSyntax> CompilationUnits { get; }

        public FixedDictionary<Name, Package> References { get; }

        public IEnumerable<Diagnostic> Diagnostics =>
            CompilationUnits.SelectMany(cu => cu.Diagnostics);

        public PackageSyntax(
            Name name,
            FixedList<ICompilationUnitSyntax> compilationUnits,
            FixedDictionary<Name, Package> references)
        {
            Symbol = new PackageSymbol(name);
            SymbolTreeBuilder = new SymbolTreeBuilder(Symbol);
            CompilationUnits = compilationUnits;
            References = references;
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
            return $"package {Symbol.Name.Text}: {CompilationUnits.Count} Compilation Units";
        }
    }
}
