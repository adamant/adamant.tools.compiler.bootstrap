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
        [NotNull] [ItemNotNull] public IReadOnlyList<MemberDeclarationAnalysis> Declarations { get; }

        public CompilationUnitAnalysis(
            [NotNull] CompilationUnitScope globalScope,
            [NotNull][ItemNotNull] IEnumerable<MemberDeclarationAnalysis> declarations)
        {
            GlobalScope = globalScope;
            Declarations = declarations.ToReadOnlyList();
        }
    }
}
