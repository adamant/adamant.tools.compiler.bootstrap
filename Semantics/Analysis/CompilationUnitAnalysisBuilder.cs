using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class CompilationUnitAnalysisBuilder
    {
        [NotNull] private readonly NameBuilder nameBuilder;
        [NotNull] private readonly DeclarationAnalysisBuilder declarationBuilder;

        public CompilationUnitAnalysisBuilder(
            [NotNull] NameBuilder nameBuilder,
            [NotNull] DeclarationAnalysisBuilder declarationBuilder)
        {
            this.declarationBuilder = declarationBuilder;
            this.nameBuilder = nameBuilder;
        }

        [NotNull]
        [ItemNotNull]
        public IEnumerable<CompilationUnitAnalysis> Build([NotNull] PackageSyntax packageSyntax)
        {
            foreach (var compilationUnit in packageSyntax.CompilationUnits)
            {
                var globalScope = new CompilationUnitScope();
                var declarations = Build(compilationUnit, globalScope);
                yield return new CompilationUnitAnalysis(globalScope, declarations);
            }
        }

        [NotNull]
        [ItemNotNull]
        private IEnumerable<DeclarationAnalysis> Build(
            [NotNull] CompilationUnitSyntax compilationUnit,
            [NotNull] CompilationUnitScope globalScope)
        {
            Name @namespace = GlobalNamespaceName.Instance;
            LexicalScope scope = globalScope;
            if (compilationUnit.Namespace != null)
            {
                @namespace = nameBuilder.BuildName(compilationUnit.Namespace.Name) ?? @namespace;
                scope = new NamespaceScope(scope);
            }

            foreach (var declaration in compilationUnit.Declarations)
            {
                var context = new AnalysisContext(compilationUnit.CodeFile, scope);
                var analysis = declarationBuilder.Build(context, @namespace, declaration);
                if (analysis != null)
                    yield return analysis;
            }
        }
    }
}
