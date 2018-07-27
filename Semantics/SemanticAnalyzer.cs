using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analayze;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxAnnotations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticAnalyzer
    {
        private readonly SyntaxSymbolBuilder syntaxSymbolBuilder = new SyntaxSymbolBuilder();

        public Package Analyze(PackageSyntax package)
        {
            var syntaxSymbols = syntaxSymbolBuilder.Build(package);

            var annotations = new Annotations(package, syntaxSymbols);
            annotations.ValidateSymbolsAnnotations();

            var nameScopeBuilder = new NameScopeBuilder(annotations);
            Parallel.ForEach(package.SyntaxTrees.Select(t => t.Root), nameScopeBuilder.Build);

            new DeclarationBinder(annotations).BindDeclarations(package);

            annotations.ValidateAnnotations();

            var treeBuilder = new SemanticTreeBuilder(annotations);

            var compilationUnits = package.SyntaxTrees
#if RELEASE
                .AsParallel()
#endif
                .Select(tree => treeBuilder.Build(tree))
                .ToList();

            var entryPoint = FindEntryPoint(compilationUnits);
            return new Package(package, compilationUnits, entryPoint);
        }

        private static FunctionDeclaration FindEntryPoint(IEnumerable<CompilationUnit> compilationUnits)
        {
            return compilationUnits
                .SelectMany(cu => cu.Declarations.OfType<FunctionDeclaration>())
                .SingleOrDefault(f => f.Name == "main");
        }
    }
}
