using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Old.Semantics.SyntaxSymbols;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Old.Semantics
{
    internal partial class SemanticAnalysis
    {
        public const string SyntaxSymbolAttribute = "SyntaxSymbol";

        [NotNull] public PackageSyntaxSymbol PackageSyntaxSymbol => SyntaxSymbol(PackageSyntax);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public PackageSyntaxSymbol SyntaxSymbol([NotNull] PackageSyntax syntax)
        {
            return attributes.GetOrAdd(syntax, SyntaxSymbolAttribute, ComputePackageSyntaxSymbol);
        }

        private static PackageSyntaxSymbol ComputePackageSyntaxSymbol(PackageSyntax package)
        {
            var compilationUnits = package.CompilationUnits.ToList();
            var globalNamespace = new GlobalNamespaceSyntaxSymbol(compilationUnits,
                ComputeDeclarationSyntaxSymbols(compilationUnits.SelectMany(cu => cu.Declarations)));
            return new PackageSyntaxSymbol(package, globalNamespace);
        }

        [NotNull]
        [ItemNotNull]
        private static IEnumerable<IDeclarationSyntaxSymbol> ComputeDeclarationSyntaxSymbols([NotNull][ItemNotNull] IEnumerable<DeclarationSyntax> declarations)
        {
            return declarations
                .Where(d => !(d is IncompleteDeclarationSyntax))
                .Select(ComputeDeclarationSyntaxSymbol);
            //var lookup = declarations.ToLookup(d => d is CompilationUnitNamespaceSyntax);

            //return lookup[true].Cast<CompilationUnitNamespaceSyntax>().GroupBy(ns => ns.Name.Value)
            //    .Select<IGrouping<string, CompilationUnitNamespaceSyntax>, IDeclarationSyntaxSymbol>(g => throw new NotImplementedException())
            //    .Concat(lookup[false].Select(ComputeDeclarationSyntaxSymbol));
        }

        [NotNull]
        private static IDeclarationSyntaxSymbol ComputeDeclarationSyntaxSymbol([NotNull] DeclarationSyntax declaration)
        {
            switch (declaration)
            {
                case FunctionDeclarationSyntax function:
                    var variableSymbols = function.ParametersList.Nodes()
                        .Select(p => new VariableSyntaxSymbol(p, null))
                        .Concat(function.Body.DescendantsAndSelf()
                            .OfType<VariableDeclarationStatementSyntax>()
                            .Select(v => new VariableSyntaxSymbol(v, null)));
                    return new FunctionSyntaxSymbol(function, variableSymbols);

                case ClassDeclarationSyntax classDeclaration:
                    return new TypeSyntaxSymbol(classDeclaration);

                case EnumStructDeclarationSyntax enumStruct:
                    return new TypeSyntaxSymbol(enumStruct);

                default:
                    throw NonExhaustiveMatchException.For(declaration);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public ISyntaxSymbol SyntaxSymbol([NotNull] SyntaxNode syntax)
        {
            return attributes.GetOrAdd(syntax, SyntaxSymbolAttribute, ComputeSyntaxSymbol);
        }

        [NotNull]
        private ISyntaxSymbol ComputeSyntaxSymbol([NotNull] SyntaxNode syntax)
        {
            switch (syntax)
            {
                case FunctionDeclarationSyntax function:
                    var parentSymbol = SyntaxSymbol(Parent(function));
                    // Because the function could be missing a name, we have to find the correct symbol by whether we declare it
                    return parentSymbol.Children.Single(c => c.Declarations.Contains(function));
                case CompilationUnitSyntax compilationUnit:
                    var packageSymbol = SyntaxSymbol(Parent(compilationUnit));
                    return packageSymbol.GlobalNamespace;
                default:
                    throw NonExhaustiveMatchException.For(syntax);
            }
        }
    }
}
