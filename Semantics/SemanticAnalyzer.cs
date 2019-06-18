using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analyzers;
using Adamant.Tools.Compiler.Bootstrap.Semantics.BindingMutability;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DefiniteAssignment;
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
        /// <summary>
        /// Whether to store the liveness analysis for each function and method.
        /// Default Value: false
        /// </summary>
        public bool SaveLivenessAnalysis = false;

        /// <summary>
        /// Whether to store the borrow checker claims for each function and method.
        /// Default Value: false
        /// </summary>
        public bool SaveBorrowClaims = false;

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

            // Do type checking
            TypeResolver.Check(memberDeclarations, diagnostics);

#if DEBUG
            TypeResolutionValidator.Validate(memberDeclarations);
            // TODO validate that all `ReferencedSymbol` properties have been set
#endif
            // TODO the move checker should be handled by the data flow analysis and type checking
            MoveChecker.Check(memberDeclarations, diagnostics);

            ShadowChecker.Check(memberDeclarations, diagnostics);

            DataFlowAnalysis.Check(DefiniteAssignmentStrategy.Instance, memberDeclarations, diagnostics);

            DataFlowAnalysis.Check(BindingMutabilityStrategy.Instance, memberDeclarations, diagnostics);

            // --------------------------------------------------
            // This is where the representation transitions to IR
            ControlFlowAnalyzer.BuildGraphs(memberDeclarations);
            // --------------------------------------------------

            var liveness = LivenessAnalyzer.Analyze(memberDeclarations, SaveLivenessAnalysis);

            // TODO inserting deletes based only on liveness led to issues.
            // There are cases when a variable is no longer directly used, but another variable has borrowed the value.
            // The owner then shows as dead, so we inserted the delete. But really, we needed to wait until the
            // borrow is gone to delete it. However, we can't just wait for all borrows to be dead to insert a delete
            // because things are guaranteed to be deleted when the owner goes out of scope. A borrow that extends
            // beyond the scope should be an error. We had been detecting this by noticing the borrow claim extended
            // past the delete statement. That worked, because the owning variable must always be dead after the scope
            // because there are no references to it outside the scope.
            //DeleteInserter.Transform(memberDeclarations, liveness);

            // Plan for inserting deletes:
            // ---------------------------
            // When generating the CFG, output statements for when variables go out of scope. Then track another level
            // of liveness between alive and dead. This would be a new state, perhaps called "liminal" or "pending", which
            // indicated that the variable would not be used again, but may need to exist as the owner of something borrowed
            // until all outstanding borrows are resolved. Delete statements could then be inserted after borrow checking
            // at the point where values are no longer used. The variable leaving scope statements could then be ignored
            // or removed.
            BorrowChecker.Check(memberDeclarations, liveness, diagnostics, SaveBorrowClaims);

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
