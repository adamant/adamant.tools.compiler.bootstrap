using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class ScopeBinder
    {
        [NotNull] [ItemNotNull] private readonly IReadOnlyList<DeclarationAnalysis> declarationAnalyses;
        [NotNull] private readonly Dictionary<string, DeclarationAnalysis> globalDeclarations;

        public ScopeBinder([NotNull][ItemNotNull] IEnumerable<DeclarationAnalysis> declarationAnalyses)
        {
            this.declarationAnalyses = declarationAnalyses.ToReadOnlyList();
            globalDeclarations = this.declarationAnalyses.Where(IsGlobalDeclaration)
                .ToDictionary(d => d.QualifiedName.Name.Text, d => d);
        }

        private static bool IsGlobalDeclaration([NotNull] DeclarationAnalysis declaration)
        {
            return !declaration.QualifiedName.Qualifier.Any();
        }

        public void Bind(CompilationUnitScope scope)
        {
            scope.Bind(globalDeclarations);
        }
    }
}
