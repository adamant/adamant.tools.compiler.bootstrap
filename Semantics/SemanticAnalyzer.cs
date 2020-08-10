using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Basic;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Errors;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ILGen;
using Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Liveness;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Scopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols.Entities;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols.Namespaces;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Validation;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.BindingMutability;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.DefiniteAssignment;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.Moves;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.Shadowing;
using Adamant.Tools.Compiler.Bootstrap.Symbols.Trees;

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
        /// Whether to store the reachability graphs for each function and method.
        /// Default Value: false
        /// </summary>
        public bool SaveReachabilityGraphs { get; set; } = false;

        [SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "OO")]
        public Package Check(PackageSyntax package)
        {
            // First pull over all the lexer and parser warnings
            var diagnostics = new Diagnostics(package.Diagnostics);

            // If there are errors from the lex and parse phase, don't continue on
            diagnostics.ThrowIfFatalErrors();

            NamespaceSymbolBuilder.BuildNamespaceSymbols(package);

            // Build up lexical scopes down to the declaration level
            new DeclarationLexicalScopesBuilder().BuildFor(package);

            // TODO remove old scopes builder
            var stringSymbol = BuildScopes(package, diagnostics);

            // Make a list of all the entity declarations (i.e. not namespaces)
            var entities = GetEntityDeclarations(package);

            CheckSemantics(entities, stringSymbol, diagnostics, package.SymbolTreeBuilder);

            // If there are errors from the semantics phase, don't continue on
            diagnostics.ThrowIfFatalErrors();

            // --------------------------------------------------
            // This is where the representation transitions to IR
            var declarations = BuildIL(entities);
            // --------------------------------------------------

            // If there are errors from the previous phase, don't continue on
            diagnostics.ThrowIfFatalErrors();

            var entryPoint = DetermineEntryPoint(declarations, diagnostics);

            var references = package.References.Values.ToFixedList();
            return new Package(package.SymbolTreeBuilder.Build(), diagnostics.Build(), references, declarations, entryPoint);
        }

        private static ITypeMetadata? BuildScopes(
            PackageSyntax package,
            Diagnostics diagnostics)
        {
            var scopesBuilder = new PackageScopesBuilder(package, diagnostics);
            scopesBuilder.BuildScopesFor(package);
            var stringSymbol = scopesBuilder.GlobalScope.LookupMetadataInGlobalScope(new SimpleName("String"))
                                            .OfType<ITypeMetadata>().FirstOrDefault();
            if (stringSymbol is null)
                // TODO we are assuming there is a compilation unit. This should be generated against the package itself
                diagnostics.Add(SemanticError.NoStringTypeDefined(package.CompilationUnits[0].CodeFile));
            return stringSymbol;
        }

        private static FixedList<IEntityDeclarationSyntax> GetEntityDeclarations(
            PackageSyntax packageSyntax)
        {
            return packageSyntax.CompilationUnits
                .SelectMany(cu => cu.AllEntityDeclarations)
                .ToFixedList();
        }

        private static void CheckSemantics(
            FixedList<IEntityDeclarationSyntax> entities,
            ITypeMetadata? stringSymbol,
            Diagnostics diagnostics,
            SymbolTreeBuilder symbolTree)
        {
            // Resolve symbols for the entities
            new EntitySymbolResolver(diagnostics, symbolTree).Resolve(entities);

            // Build lexical sco

            // Basic Analysis includes: Name Binding, Type Checking, Constant Folding
            new BasicAnalyzer(stringSymbol, diagnostics).Check(entities);

            // If there are errors from the basic analysis phase, don't continue on
            diagnostics.ThrowIfFatalErrors();

#if DEBUG
            new SymbolValidator(symbolTree).Walk(entities);
            new TypeFulfillmentValidator().Walk(entities);
            new ReferencedSymbolValidator().Walk(entities);
            new TypeKnownValidator().Walk(entities);
            new ExpressionSemanticsValidator().Walk(entities);
#endif

            // From this point forward, analysis focuses on callable bodies
            // TODO what about field initializers?
            var callables = entities.OfType<IConcreteCallableDeclarationSyntax>().ToFixedList();

            ShadowChecker.Check(callables, diagnostics);

            DataFlowAnalysis.Check(DefiniteAssignmentAnalyzer.Instance, callables, diagnostics);

            DataFlowAnalysis.Check(BindingMutabilityAnalyzer.Instance, callables, diagnostics);

            DataFlowAnalysis.Check(UseOfMovedValueAnalyzer.Instance, callables, diagnostics);

            // TODO use DataFlowAnalysis to check for unused variables and report use of variables starting with `_`

            // Compute variable liveness needed by reachability analyzer
            DataFlowAnalysis.Check(LivenessAnalyzer.Instance, callables, diagnostics);

            ReachabilityAnalyzer.Analyze(callables, diagnostics);

            // TODO remove live variables if SaveLivenessAnalysis is false
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
