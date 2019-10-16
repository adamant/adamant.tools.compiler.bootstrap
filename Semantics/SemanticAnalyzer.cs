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
        public bool SaveLivenessAnalysis { get; set; } = false;

        /// <summary>
        /// Whether to store the borrow checker claims for each function and method.
        /// Default Value: false
        /// </summary>
        public bool SaveBorrowClaims { get; set; } = false;

        public Package Check(
            PackageSyntax packageSyntax,
            FixedDictionary<string, Package> references)
        {
            // First pull over all the lexer and parser warnings
            var diagnostics = new Diagnostics(packageSyntax.Diagnostics);

            // If there are errors from the lex and parse phase, don't continue on
            diagnostics.ThrowIfFatalErrors();

            BuildLexicalScopes(packageSyntax, references, diagnostics);

            // Make a list of all the entity declarations (i.e. not namespaces)
            var entityDeclarations = GetEntityDeclarations(packageSyntax);

            CheckSemantics(entityDeclarations, diagnostics);

            // If there are errors from the semantics phase, don't continue on
            diagnostics.ThrowIfFatalErrors();

            // --------------------------------------------------
            // This is where the representation transitions to IR
            var ilDeclarations = BuildIL(entityDeclarations);
            // --------------------------------------------------

            var ilCallables = ilDeclarations.OfType<ICallableDeclaration>().ToFixedList();

            var liveness = LivenessAnalyzer.Check(ilCallables, SaveLivenessAnalysis);

            BorrowChecker.Check(ilCallables, liveness, diagnostics, SaveBorrowClaims);

            // If there are errors from the previous phase, don't continue on
            diagnostics.ThrowIfFatalErrors();

            var entryPoint = DetermineEntryPoint(ilDeclarations, diagnostics);

            return new Package(packageSyntax.Name, diagnostics.Build(), references, ilDeclarations, entryPoint);
        }

        private static void BuildLexicalScopes(
            PackageSyntax packageSyntax,
            FixedDictionary<string, Package> references,
            Diagnostics diagnostics)
        {
            var scopesBuilder = new LexicalScopesBuilder(diagnostics, packageSyntax, references);
            scopesBuilder.BuildScopesInPackage(packageSyntax);
        }

        private static FixedList<IEntityDeclarationSyntax> GetEntityDeclarations(
            PackageSyntax packageSyntax)
        {
            return packageSyntax.CompilationUnits
                .SelectMany(cu => cu.EntityDeclarations)
                .ToFixedList();
        }

        private static void CheckSemantics(
            FixedList<IEntityDeclarationSyntax> entityDeclarations,
            Diagnostics diagnostics)
        {
            // Basic Analysis includes: Name Binding, Type Checking, Constant Folding
            BasicAnalyzer.Check(entityDeclarations, diagnostics);

#if DEBUG
            TypeFulfillmentValidator.Validate(entityDeclarations);
            new NoUpgradableMutabilityTypesValidator().Walk(entityDeclarations);
            ReferencedSymbolValidator.Validate(entityDeclarations);
#endif

            // From this point forward, analysis focuses on functions
            var callableDeclarations =
                entityDeclarations.OfType<ICallableDeclarationSyntax>().ToFixedList();
            ShadowChecker.Check(callableDeclarations, diagnostics);

            // TODO use DataFlowAnalysis to check for unused variables and report use of variables starting with `_`

            DataFlowAnalysis.Check(DefiniteAssignmentStrategy.Instance, callableDeclarations,
                diagnostics);

            DataFlowAnalysis.Check(BindingMutabilityStrategy.Instance, callableDeclarations,
                diagnostics);

            DataFlowAnalysis.Check(UseOfMovedValueStrategy.Instance, callableDeclarations,
                diagnostics);
        }

        private static FixedList<Declaration> BuildIL(
            FixedList<IEntityDeclarationSyntax> entityDeclarations)
        {
            // TODO construct IL while building control flow graphs, then analyze borrow check on that

            var controlFlowGraphFactory = new ControlFlowGraphFactory();
            var declarationBuilder = new DeclarationBuilder(controlFlowGraphFactory);
            declarationBuilder.Build(entityDeclarations);
            return declarationBuilder.AllDeclarations.ToFixedList();
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
