using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxAnnotations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticAnalyzer
    {
        public Package Analyze(PackageSyntax package)
        {
            var annotations = BuildSyntaxSymbols(package);

            BuildNameScopes(package, annotations);

            BindAnnotations(package, annotations);

            var compilationUnits = package.SyntaxTrees
#if RELEASE
                .AsParallel()
#endif
                .Select(t => Analyze(t.Root, annotations))
                .ToList();

            var entryPoint = FindEntryPoint(compilationUnits);
            return new Package(package, compilationUnits, entryPoint);
        }

        private static Annotations BuildSyntaxSymbols(PackageSyntax package)
        {
            var syntaxSymbols = new SyntaxSymbolBuilder().Build(package);

            var annotations = new Annotations(package, syntaxSymbols);
            annotations.ValidateSymbolsAnnotations();
            return annotations;
        }

        private static void BuildNameScopes(PackageSyntax package, Annotations annotations)
        {
            var nameScopeBuilder = new NameScopeBuilder(annotations);
            Parallel.ForEach(package.SyntaxTrees.Select(t => t.Root), nameScopeBuilder.Build);
            annotations.ValidateNameScopeAnnotations();
        }

        private static void BindAnnotations(PackageSyntax package, Annotations annotations)
        {
            var annotationBinder = new AnnotationBinder(annotations);
            annotationBinder.Bind(package);
            annotations.ValidateAnnotationBindings();
        }

        private static CompilationUnit Analyze(CompilationUnitSyntax compilationUnit, Annotations annotations)
        {
            var treeBuilder = new SemanticTreeBuilder(annotations);
            return treeBuilder.Build(compilationUnit);
        }

        private static FunctionDeclaration FindEntryPoint(IEnumerable<CompilationUnit> compilationUnits)
        {
            return compilationUnits
                .SelectMany(cu => cu.Declarations.OfType<FunctionDeclaration>())
                .SingleOrDefault(f => f.Name == "main");
        }
    }
}
