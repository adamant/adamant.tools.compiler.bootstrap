using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class CompilationUnitAnalysis
    {
        [NotNull] public CompilationUnitScope GlobalScope { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<DeclarationAnalysis> Declarations { get; }

        public CompilationUnitAnalysis(
            [NotNull] CompilationUnitScope globalScope,
            [NotNull][ItemNotNull] IEnumerable<DeclarationAnalysis> declarations)
        {
            GlobalScope = globalScope;
            Declarations = declarations.ToReadOnlyList();
        }
    }
}
