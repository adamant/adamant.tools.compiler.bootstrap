using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes
{
    public class CompilationUnitScope : LexicalScope
    {
        [NotNull] public new CompilationUnitSyntax Syntax { get; }
        [CanBeNull] private IReadOnlyDictionary<string, DeclarationAnalysis> globalDeclarations;

        public CompilationUnitScope([NotNull] CompilationUnitSyntax compilationUnit)
            : base(compilationUnit)
        {
            Syntax = compilationUnit;
        }

        public void Bind([NotNull] Dictionary<string, DeclarationAnalysis> globalDeclarations)
        {
            Requires.NotNull(nameof(globalDeclarations), globalDeclarations);
            this.globalDeclarations = new Dictionary<string, DeclarationAnalysis>(globalDeclarations).AsReadOnly();
        }

        [CanBeNull]
        public override DeclarationAnalysis Lookup(string name)
        {
            Requires.NotNull(nameof(name), name);
            return globalDeclarations.AssertNotNull().TryGetValue(name, out var declaration) ? declaration : null;
        }
    }
}
