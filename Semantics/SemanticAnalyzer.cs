using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Deletes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Liveness;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Moving;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Shadowing;
using Adamant.Tools.Compiler.Bootstrap.Semantics.TypeChecking;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Validation;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    public class SemanticAnalyzer
    {
        public Package Analyze(
            PackageSyntax packageSyntax,
            FixedDictionary<string, Package> references)
        {
            // First pull over all the lexer and parser errors from the compilation units
            var diagnostics = AllDiagnostics(packageSyntax);

            var scopesBuilder = new LexicalScopesBuilder(diagnostics, packageSyntax, references);
            scopesBuilder.BuildScopesInPackage(packageSyntax);

            // Make a list of all the member declarations (i.e. not namespaces)
            var memberDeclarations = packageSyntax.CompilationUnits
                .SelectMany(cu => cu.AllMemberDeclarations).ToFixedList();

            // TODO we can't do full type checking without some IL gen and code execution, how to handle that?

            // Do type checking
            TypeResolver.Check(memberDeclarations, diagnostics);

#if DEBUG
            TypeResolutionValidator.Validate(memberDeclarations);
            // TODO validate that all ReferencedSymbols lists have a single value non-errored code
#endif
            MoveChecker.Check(memberDeclarations, diagnostics);

            ShadowChecker.Check(memberDeclarations, diagnostics);

            // TODO we need to check definite assignment as well

            // --------------------------------------------------
            // This is where the representation transitions to IR
            ControlFlowAnalyzer.BuildGraphs(memberDeclarations);
            // --------------------------------------------------

            var liveness = LivenessAnalyzer.Analyze(memberDeclarations);

            DeleteInserter.Transform(memberDeclarations, liveness);

            BorrowChecker.Check(memberDeclarations, diagnostics);

            // Build final declaration objects and find the entry point
            var declarationBuilder = new DeclarationBuilder();
            var declarations = declarationBuilder.Build(memberDeclarations);
            var entryPoint = DetermineEntryPoint(declarations, diagnostics);

            return new Package(packageSyntax.Name, diagnostics.Build(), references, declarations, entryPoint);
        }

        private static Diagnostics AllDiagnostics(PackageSyntax packageSyntax)
        {
            var diagnostics = new Diagnostics();
            foreach (var compilationUnit in packageSyntax.CompilationUnits)
                diagnostics.Add(compilationUnit.Diagnostics);
            return diagnostics;
        }

        private static FunctionDeclaration DetermineEntryPoint(
            FixedList<Declaration> declarations,
            Diagnostics diagnostics)
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
