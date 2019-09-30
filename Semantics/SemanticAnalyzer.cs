using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Basic;
using Adamant.Tools.Compiler.Bootstrap.Semantics.BindingMutability;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Borrowing;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Builders;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DefiniteAssignment;
using Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Liveness;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Moves;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Shadowing;
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

        public Package Check(
            PackageSyntax packageSyntax,
            FixedDictionary<string, Package> references)
        {
            // First pull over all the lexer and parser warnings
            var diagnostics = new Diagnostics(packageSyntax.Diagnostics);

            // If there are errors from the previous phase, don't continue on
            diagnostics.ThrowIfFatalErrors();

            var scopesBuilder = new LexicalScopesBuilder(diagnostics, packageSyntax, references);
            scopesBuilder.BuildScopesInPackage(packageSyntax);

            // Make a list of all the entity declarations (i.e. not namespaces)
            var entityDeclarations = packageSyntax.CompilationUnits
                .SelectMany(cu => cu.EntityDeclarations).ToFixedList();

            // Basic Analysis includes: Name Binding, Type Checking, Constant Folding
            BasicAnalyzer.Check(entityDeclarations, diagnostics);

#if DEBUG
            TypeFulfillmentValidator.Validate(entityDeclarations);
            NoUpgradableMutabilityTypesValidator.Validate(entityDeclarations);
            ReferencedSymbolValidator.Validate(entityDeclarations);
#endif

            ShadowChecker.Check(entityDeclarations, diagnostics);

            // TODO use DataFlowAnalysis to check for unused variables and report use of variables starting with `_`

            DataFlowAnalysis.Check(DefiniteAssignmentStrategy.Instance, entityDeclarations, diagnostics);

            DataFlowAnalysis.Check(BindingMutabilityStrategy.Instance, entityDeclarations, diagnostics);

            DataFlowAnalysis.Check(UseOfMovedValueStrategy.Instance, entityDeclarations, diagnostics);

            // If there are errors from the previous phase, don't continue on
            diagnostics.ThrowIfFatalErrors();

            // --------------------------------------------------
            // This is where the representation transitions to IR
            ControlFlowAnalyzer.BuildGraphs(entityDeclarations);
            // --------------------------------------------------

            var liveness = LivenessAnalyzer.Check(entityDeclarations, SaveLivenessAnalysis);

            BorrowChecker.Check(entityDeclarations, liveness, diagnostics, SaveBorrowClaims);

            // If there are errors from the previous phase, don't continue on
            diagnostics.ThrowIfFatalErrors();

            // Build final declaration objects and find the entry point
            var declarationBuilder = new DeclarationBuilder();
            declarationBuilder.Build(entityDeclarations);
            var declarations = declarationBuilder.AllDeclarations.ToFixedList();
            var entryPoint = DetermineEntryPoint(declarations, diagnostics);

            return new Package(packageSyntax.Name, diagnostics.Build(), references, declarations, entryPoint);
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
