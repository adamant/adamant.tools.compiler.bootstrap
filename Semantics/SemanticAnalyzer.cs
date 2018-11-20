using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Model;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticAnalyzer
    {
        [NotNull]
        public Package Analyze(
            [NotNull] PackageSyntax packageSyntax,
            [NotNull] FixedDictionary<string, Package> references)
        {
            // First pull over all the lexer and parser errors from the compilation units
            var diagnostics = AllDiagnostics(packageSyntax);
            //var nameBuilder = new NameBuilder();

            // TODO do we need a list of all the namespaces for validating using statements?
            // Gather a list of all the namespaces for validating using statements

            var nameBinder = new NameBinder(packageSyntax, references);
            nameBinder.BindNames(packageSyntax);

            // Make a list of all the non-member declarations
            var nonMemberDeclarations = packageSyntax.CompilationUnits
                .SelectMany(cu => cu.AllNonMemberDeclarations).ToFixedList();

            // Build final declaration objects and find the entry point
            var declarationBuilder = new DeclarationBuilder();
            var declarations = declarationBuilder.Build(nonMemberDeclarations);
            var entryPoint = DetermineEntryPoint(declarations, diagnostics);

            return new Package(packageSyntax.Name, diagnostics.Build(), references, declarations, entryPoint);

            // -----------------------------------------------
            // Old Analysis

            //// Gather all the declarations and simultaneously build up trees of lexical scopes
            //var compilationUnits = new AnalysisBuilder().Build(packageSyntax).ToList();

            //// Check lexical scopes and attach to entities etc.
            ////var oldScopeBinder = new OldScopeBinder(compilationUnits, nameBuilder, references);
            ////foreach (var scope in compilationUnits.Select(cu => cu.GlobalScope))
            ////    oldScopeBinder.BindCompilationUnitScope(scope);

            //// Make a list of all the member declarations
            //var declarationAnalyses = compilationUnits.SelectMany(cu => cu.MemberDeclarations).ToList();

            //// Do name binding, type checking, IL statement generation and compile time code execution
            //// They are all interdependent to some degree
            //TypeChecker.CheckDeclarations(declarationAnalyses);

            //// At this point, some but not all of the functions will have IL statements generated,
            //// now generate the rest
            //ControlFlowGraphBuilder.BuildGraphs(declarationAnalyses.OfType<FunctionDeclarationAnalysis>());

            //// Only borrow checking left
            //var borrowChecker = new BorrowChecker();
            //borrowChecker.Check(declarationAnalyses);

            //// Gather the diagnostics and declarations into a package
            //var diagnostics = new Diagnostics();
            //// First pull over all the lexer and parser errors from the compilation units
            //foreach (var compilationUnit in packageSyntax.CompilationUnits)
            //    diagnostics.Add(compilationUnit.Diagnostics);

            //var declarations = declarationAnalyses.Select(d => d.Complete(diagnostics)).Where(d => d != null).ToList();
            //var entryPoint = DetermineEntryPoint(declarations, diagnostics);
            //return new Package(packageSyntax.Name, diagnostics.Build(), references, declarations, entryPoint);
        }

        [NotNull]
        private static Diagnostics AllDiagnostics([NotNull] PackageSyntax packageSyntax)
        {
            var diagnostics = new Diagnostics();
            foreach (var compilationUnit in packageSyntax.CompilationUnits)
                diagnostics.Add(compilationUnit.Diagnostics);
            return diagnostics;
        }

        [CanBeNull]
        private static FunctionDeclaration DetermineEntryPoint(
            [NotNull] FixedList<Declaration> declarations,
            [NotNull] Diagnostics diagnostics)
        {
            var mainFunctions = declarations.OfType<FunctionDeclaration>()
                .Where(f => f.Name.UnqualifiedName.Text == "main" && !f.Name.UnqualifiedName.IsSpecial)
                .ToList();

            // TODO warn on and remove main functions that don't have correct parameters or types

            // TODO compiler error on multiple main functions

            return mainFunctions.SingleOrDefault();
        }
    }
}
