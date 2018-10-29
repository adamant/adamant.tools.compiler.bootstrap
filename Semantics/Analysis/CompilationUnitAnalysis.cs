using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class CompilationUnitAnalysis : AnalysisNode
    {
        [NotNull] public CompilationUnitScope GlobalScope { get; }
        [NotNull] public new CompilationUnitSyntax Syntax { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<MemberDeclarationAnalysis> Declarations { get; }

        public CompilationUnitAnalysis(
            [NotNull] CompilationUnitScope globalScope,
            [NotNull] CompilationUnitSyntax syntax,
            [NotNull] [ItemNotNull] IEnumerable<MemberDeclarationAnalysis> declarations)
            : base(new AnalysisContext(syntax.CodeFile, globalScope), syntax)
        {
            Requires.NotNull(nameof(globalScope), globalScope);
            Requires.NotNull(nameof(declarations), declarations);
            GlobalScope = globalScope;
            Syntax = syntax;
            Declarations = declarations.ToReadOnlyList();
        }
    }
}
