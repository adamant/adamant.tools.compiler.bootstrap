using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.NameBinding;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Validation;
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

            var nameBinder = new NameBinder(diagnostics, packageSyntax, references);
            nameBinder.BindNamesInPackage(packageSyntax);

            // Make a list of all the member declarations (i.e. not namespaces)
            var memberDeclarations = packageSyntax.CompilationUnits
                .SelectMany(cu => cu.AllMemberDeclarations).ToFixedList();

            // TODO we can't do full type checking without some IL gen and code execution, how to handle that?

            // Do type checking
            var typeChecker = new DeclarationTypeResolver(diagnostics);
            typeChecker.ResolveTypesInDeclarations(memberDeclarations);

#if DEBUG
            TypeResolutionValidator.Validate(memberDeclarations);
#endif

            ControlFlowGraphBuilder.BuildGraphs(memberDeclarations);

            //var borrowChecker = new BorrowChecker();
            //borrowChecker.Check(declarationAnalyses);

            // Build final declaration objects and find the entry point
            var declarationBuilder = new DeclarationBuilder();
            var declarations = declarationBuilder.Build(memberDeclarations);
            var entryPoint = DetermineEntryPoint(declarations, diagnostics);

            return new Package(packageSyntax.Name, diagnostics.Build(), references, declarations, entryPoint);
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
                .Where(f => f.FullName.UnqualifiedName.Text == "main" && !f.FullName.UnqualifiedName.IsSpecial)
                .ToList();

            // TODO warn on and remove main functions that don't have correct parameters or types

            // TODO compiler error on multiple main functions

            return mainFunctions.SingleOrDefault();
        }
    }
}
