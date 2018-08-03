using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    internal partial class SemanticAnalysis
    {
        public const string SyntaxSymbolAttribute = "SyntaxSymbol";

        public PackageSyntaxSymbol PackageSyntaxSymbol => SyntaxSymbol(PackageSyntax);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PackageSyntaxSymbol SyntaxSymbol(PackageSyntax syntax)
        {
            return attributes.GetOrAdd(syntax, SyntaxSymbolAttribute, ComputePackageSyntaxSymbol);
        }

        private PackageSyntaxSymbol ComputePackageSyntaxSymbol(PackageSyntax package)
        {
            var compilationUnits = package.CompilationUnits.ToList();
            var globalNamespace = new GlobalNamespaceSyntaxSymbol(compilationUnits,
                ComputeDeclarationSyntaxSymbols(compilationUnits.SelectMany(cu => cu.Children.OfType<DeclarationSyntax>())));
            return new PackageSyntaxSymbol(package, globalNamespace);
        }

        private IEnumerable<IDeclarationSyntaxSymbol> ComputeDeclarationSyntaxSymbols(IEnumerable<DeclarationSyntax> declarations)
        {
            var lookup = declarations.ToLookup(d => d is NamespaceSyntax);

            return lookup[true].Cast<NamespaceSyntax>().GroupBy(ns => ns.Name.Value)
                .Select<IGrouping<string, NamespaceSyntax>, IDeclarationSyntaxSymbol>(g => throw new NotImplementedException())
                .Concat(lookup[false].Select(ComputeDeclarationSyntaxSymbol));
        }

        private IDeclarationSyntaxSymbol ComputeDeclarationSyntaxSymbol(DeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case FunctionDeclarationSyntax function:
                    var variableSymbols = function.Parameters
                        .Select(p => new VariableSyntaxSymbol(p, null))
                        .Concat(function.Body.DecendantStatementsAndSelf()
                            .OfType<VariableDeclarationStatementSyntax>()
                            .Select(v => new VariableSyntaxSymbol(v, null)));
                    return new FunctionSyntaxSymbol(function, variableSymbols);

                case ClassDeclarationSyntax classDeclaration:
                    return new TypeSyntaxSymbol(classDeclaration);

                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ISyntaxSymbol SyntaxSymbol(SyntaxBranchNode syntax)
        {
            return attributes.GetOrAdd(syntax, SyntaxSymbolAttribute, ComputeSyntaxSymbol);
        }

        private ISyntaxSymbol ComputeSyntaxSymbol(SyntaxBranchNode syntax)
        {
            switch (syntax)
            {
                case FunctionDeclarationSyntax function:
                    var parentSymbol = SyntaxSymbol(Parent(function));
                    return parentSymbol.Children.Single(c => c.Name == function.Name.Value);
                case CompilationUnitSyntax compilationUnit:
                    var packageSymbol = SyntaxSymbol(Parent(compilationUnit));
                    return packageSymbol.GlobalNamespace;
                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }
        }
    }
}
