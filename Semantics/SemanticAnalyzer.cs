using System.Collections.Generic;
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
            var package = new Package(packageSyntax.Name);

            var nameBuilder = new NameBuilder();

            // Gather a list of all the namespaces for validating using statements
            new NamespaceBuilder(nameBuilder).GatherNamespaces(package, packageSyntax);

            // Gather all the declarations and simultaneously build up trees of lexical scopes
            var compilationUnits = BuildCompilationUnits(packageSyntax, nameBuilder);

            // Check lexical namespaces and attach to entities etc.
            var scopeBinder = new ScopeBinder();
            foreach (var scope in compilationUnits.Select(cu => cu.GlobalScope))
                scopeBinder.Bind(scope);

            var declarations = compilationUnits.SelectMany(cu => cu.Declarations).ToList();

            // Do name binding, type checking, IL statement generation and compile time code execution
            // They are all interdependent to some degree
            var expressionAnalysisBuilder = new ExpressionAnalysisBuilder();
            var nameBinder = new NameBinder();
            var typeChecker = new TypeChecker(nameBinder, expressionAnalysisBuilder);
            typeChecker.CheckTypes(declarations);

            // At this point, some but not all of the functions will have IL statements generated
            var cfgBuilder = new ControlFlowGraphBuilder();
            cfgBuilder.BuildGraph(declarations.OfType<FunctionDeclarationAnalysis>());

            // Only borrow checking left
            var borrowChecker = new BorrowChecker();
            borrowChecker.Check(package);

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

        [NotNull]
        [ItemNotNull]
        private static List<CompilationUnitAnalysis> BuildCompilationUnits(
            [NotNull] PackageSyntax packageSyntax,
            [NotNull] NameBuilder nameBuilder)
        {
            var expressionBuilder = new ExpressionAnalysisBuilder();
            var statementBuilder = new StatementAnalysisBuilder(expressionBuilder);
            var declarationBuilder = new DeclarationAnalysisBuilder(nameBuilder, expressionBuilder, statementBuilder);
            var compilationUnitBuilder = new CompilationUnitAnalysisBuilder(nameBuilder, declarationBuilder);
            return compilationUnitBuilder.Build(packageSyntax).ToList();
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
