using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Basic;
using Adamant.Tools.Compiler.Bootstrap.Semantics.BindingMutability;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Builders;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DefiniteAssignment;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ILGen;
using Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes;
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

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
        public Package Check(
            PackageSyntax packageSyntax,
            FixedDictionary<string, Package> references)
        {
            // First pull over all the lexer and parser warnings
            var diagnostics = new Diagnostics(packageSyntax.Diagnostics);

            // If there are errors from the lex and parse phase, don't continue on
            diagnostics.ThrowIfFatalErrors();

            var stringSymbol = BuildLexicalScopes(packageSyntax, references, diagnostics);

            // Make a list of all the entity declarations (i.e. not namespaces)
            var entities = GetEntityDeclarations(packageSyntax);

            CheckSemantics(entities, stringSymbol, diagnostics);

            // If there are errors from the semantics phase, don't continue on
            diagnostics.ThrowIfFatalErrors();

            // --------------------------------------------------
            // This is where the representation transitions to IR
            var declarations = BuildIL(entities);
            // --------------------------------------------------

            //var callables = declarations.OfType<ICallableDeclaration>().ToFixedList();

            //var liveness = LivenessAnalyzer.Check(callables, SaveLivenessAnalysis);

            //BorrowChecker.Check(callables, liveness, diagnostics, SaveBorrowClaims);

            // If there are errors from the previous phase, don't continue on
            diagnostics.ThrowIfFatalErrors();

            var entryPoint = DetermineEntryPoint(declarations, diagnostics);

            return new Package(packageSyntax.Name, diagnostics.Build(), references, declarations, entryPoint);
        }

        private static ITypeSymbol? BuildLexicalScopes(
            PackageSyntax packageSyntax,
            FixedDictionary<string, Package> references,
            Diagnostics diagnostics)
        {
            var scopesBuilder = new PackageLexicalScopesBuilder(packageSyntax, references, diagnostics);
            scopesBuilder.BuildScopesFor(packageSyntax);
            var stringSymbol = scopesBuilder.GlobalScope.LookupInGlobalScope(new SimpleName("String"))
                                            .OfType<ITypeSymbol>().FirstOrDefault();
            if (stringSymbol is null)
                // TODO we are assuming there is a compilation unit. This should be generated against the package itself
                diagnostics.Add(SemanticError.NoStringTypeDefined(packageSyntax.CompilationUnits[0].CodeFile));
            return stringSymbol;
        }

        private static FixedList<IEntityDeclarationSyntax> GetEntityDeclarations(
            PackageSyntax packageSyntax)
        {
            return packageSyntax.CompilationUnits
                .SelectMany(cu => cu.EntityDeclarations)
                .ToFixedList();
        }

        private static void CheckSemantics(
            FixedList<IEntityDeclarationSyntax> entities,
            ITypeSymbol? stringSymbol,
            Diagnostics diagnostics)
        {
            // Basic Analysis includes: Name Binding, Type Checking, Constant Folding
            new BasicAnalyzer(stringSymbol, diagnostics).Check(entities);

            // If there are errors from the basic analysis phase, don't continue on
            diagnostics.ThrowIfFatalErrors();

#if DEBUG
            new TypeFulfillmentValidator().Walk(entities);
            new ReferencedSymbolValidator().Walk(entities);
            new TypeKnownValidator().Walk(entities);
#endif

            // From this point forward, analysis focuses on callable bodies
            var callables = entities.OfType<IConcreteCallableDeclarationSyntax>().ToFixedList();
            ShadowChecker.Check(callables, diagnostics);

            // TODO use DataFlowAnalysis to check for unused variables and report use of variables starting with `_`

            DataFlowAnalysis.Check(DefiniteAssignmentStrategy.Instance, callables, diagnostics);

            DataFlowAnalysis.Check(BindingMutabilityStrategy.Instance, callables, diagnostics);

            DataFlowAnalysis.Check(UseOfMovedValueStrategy.Instance, callables, diagnostics);
        }

        private static FixedList<Declaration> BuildIL(
            FixedList<IEntityDeclarationSyntax> entityDeclarations)
        {
            var ilFactory = new ILFactory();
            var declarationBuilder = new DeclarationBuilder(ilFactory);
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
            _ = diagnostics;
            // TODO compiler error on multiple main functions

            return mainFunctions.SingleOrDefault();
        }
    }
}
