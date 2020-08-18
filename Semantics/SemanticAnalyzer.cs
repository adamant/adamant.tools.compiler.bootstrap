using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage;
using Adamant.Tools.Compiler.Bootstrap.Semantics.AST;
using Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Basic;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow;
using Adamant.Tools.Compiler.Bootstrap.Semantics.DeclarationNumbers;
using Adamant.Tools.Compiler.Bootstrap.Semantics.ILGen;
using Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Liveness;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols.Entities;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Symbols.Namespaces;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Validation;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.BindingMutability;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.DefiniteAssignment;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.Moves;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Variables.Shadowing;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

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
        public PackageIL Check(PackageSyntax packageSyntax)
        {
            // If there are errors from the lex and parse phase, don't continue on
            packageSyntax.Diagnostics.ThrowIfFatalErrors();

            NamespaceSymbolBuilder.BuildNamespaceSymbols(packageSyntax);

            // Build up lexical scopes down to the declaration level
            new LexicalScopesBuilder().BuildFor(packageSyntax);

            // Check the semantics of the package
            var packageAbstractSyntax = CheckSemantics(packageSyntax);

            // If there are errors from the semantics phase, don't continue on
            packageAbstractSyntax.Diagnostics.ThrowIfFatalErrors();

            // --------------------------------------------------
            // This is where the representation transitions to IR
            var declarationsIL = BuildIL(packageAbstractSyntax);
            // --------------------------------------------------

            // If there are errors from the previous phase, don't continue on
            // TODO can the BuildIL() step introduce errors?
            packageAbstractSyntax.Diagnostics.ThrowIfFatalErrors();

            var entryPointIL = DetermineEntryPoint(declarationsIL, packageAbstractSyntax.Diagnostics);

            var references = packageSyntax.ReferencedPackages.ToFixedSet();
            return new PackageIL(packageAbstractSyntax.SymbolTree, packageAbstractSyntax.Diagnostics.Build(), references, declarationsIL, entryPointIL);
        }

        private static Package CheckSemantics(PackageSyntax packageSyntax)
        {
            DeclarationNumberAssigner.AssignIn(packageSyntax.AllEntityDeclarations);

            // Resolve symbols for the entities
            EntitySymbolBuilder.BuildFor(packageSyntax);

            var stringSymbol = packageSyntax.SymbolTrees
                                            .GlobalSymbols
                                            .OfType<ObjectTypeSymbol>()
                                            .SingleOrDefault(s => s.Name == "String");

            // Basic Analysis includes: Name Binding, Type Checking, Constant Folding
            BasicAnalyzer.Check(packageSyntax, stringSymbol);

            // If there are errors from the basic analysis phase, don't continue on
            packageSyntax.Diagnostics.ThrowIfFatalErrors();

#if DEBUG
            new SymbolValidator(packageSyntax.SymbolTree).Validate(packageSyntax.AllEntityDeclarations);
            new TypeFulfillmentValidator().Validate(packageSyntax.AllEntityDeclarations);
            new TypeKnownValidator().Validate(packageSyntax.AllEntityDeclarations);
            new ExpressionSemanticsValidator().Validate(packageSyntax.AllEntityDeclarations);
#endif

            var package = new ASTBuilder().BuildPackage(packageSyntax);

            // From this point forward, analysis focuses on executable declarations (i.e. invocables and field initializers)
            var executableDeclarations = package.AllDeclarations.OfType<IExecutableDeclaration>().ToFixedSet();

            ShadowChecker.Check(executableDeclarations, package.Diagnostics);

            DataFlowAnalysis.Check(DefiniteAssignmentAnalyzer.Instance, executableDeclarations, package.SymbolTree, package.Diagnostics);

            DataFlowAnalysis.Check(BindingMutabilityAnalyzer.Instance, executableDeclarations, package.SymbolTree, package.Diagnostics);

            DataFlowAnalysis.Check(UseOfMovedValueAnalyzer.Instance, executableDeclarations, package.SymbolTree, package.Diagnostics);

            // TODO use DataFlowAnalysis to check for unused variables and report use of variables starting with `_`

            // Compute variable liveness needed by reachability analyzer
            DataFlowAnalysis.Check(LivenessAnalyzer.Instance, executableDeclarations, package.SymbolTree, package.Diagnostics);

            ReachabilityAnalyzer.Analyze(executableDeclarations, package.SymbolTree, package.Diagnostics);

            // TODO remove live variables if SaveLivenessAnalysis is false

            return package;
        }

        private static FixedList<DeclarationIL> BuildIL(Package package)
        {
            var ilFactory = new ILFactory();
            var declarationBuilder = new DeclarationBuilder(ilFactory);
            declarationBuilder.Build(package.AllDeclarations, package.SymbolTree);
            return declarationBuilder.AllDeclarations.ToFixedList();
        }

        private static FunctionIL DetermineEntryPoint(
            FixedList<DeclarationIL> declarations,
            Diagnostics diagnostics)
        {
            var mainFunctions = declarations.OfType<FunctionIL>()
                .Where(f => f.Symbol.Name == "main")
                .ToList();

            // TODO warn on and remove main functions that don't have correct parameters or types
            _ = diagnostics;
            // TODO compiler error on multiple main functions

            return mainFunctions.SingleOrDefault();
        }
    }
}
