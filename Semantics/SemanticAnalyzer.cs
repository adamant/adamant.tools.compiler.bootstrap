using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticAnalyzer
    {
        public Package Analyze([NotNull] PackageSyntax packageSyntax)
        {
            // TODO change the below code so it isn't dependent on the package. Create package at end.
            var package = new Package(packageSyntax.Name);

            var nameBuilder = new NameBuilder();

            // Gather a list of all the namespaces for validating using statements
            new NamespaceBuilder(nameBuilder).GatherNamespaces(package, packageSyntax);

            // Gather all the declarations and simultaneously build up trees of lexical scopes
            var compilationUnits = new AnalysisBuilder(nameBuilder).Build(packageSyntax);

            // Check lexical scopes and attach to entities etc.
            var scopeBinder = new ScopeBinder();
            foreach (var scope in compilationUnits.Select(cu => cu.GlobalScope))
                scopeBinder.Bind(scope);

            // Make a list of tall the declarations now that they have proper scopes etc.
            var declarations = compilationUnits.SelectMany(cu => cu.Declarations).ToList();

            // Do name binding, type checking, IL statement generation and compile time code execution
            // They are all interdependent to some degree
            var expressionAnalysisBuilder = new ExpressionAnalysisBuilder();
            var nameBinder = new NameBinder();
            var typeChecker = new TypeChecker(nameBinder, expressionAnalysisBuilder);
            typeChecker.CheckTypes(declarations);

            // At this point, some but not all of the functions will have IL statements generated,
            // now generate the rest
            var cfgBuilder = new ControlFlowGraphBuilder();
            cfgBuilder.BuildGraph(declarations.OfType<FunctionDeclarationAnalysis>());

            // Only borrow checking left
            var borrowChecker = new BorrowChecker();
            borrowChecker.Check(declarations);

            // Gather the diagnostics and declarations into a package
            var diagnostics = new DiagnosticsBuilder();
            foreach (var declaration in declarations)
            {
                package.Add(declaration.Complete());
                diagnostics.Publish(declaration.Diagnostics.Build());
            }

            DetermineEntryPoint(package, diagnostics);
            package.Diagnostics = diagnostics.Build();
            return package;
        }

        private static void DetermineEntryPoint([NotNull] Package package, [NotNull] DiagnosticsBuilder diagnostics)
        {
            var mainFunctions = package.Declarations.OfType<FunctionDeclaration>()
                // TODO make an easy way to construct and compare qualified names
                .Where(f => !f.QualifiedName.Qualifier.Any()
                            && f.QualifiedName.Name.Text == "main")
                .ToList();

            // TODO warn on and remove main functions that don't have correct parameters or types

            // TODO compiler error on multiple main functions

            package.EntryPoint = mainFunctions.SingleOrDefault();
        }
    }
}
