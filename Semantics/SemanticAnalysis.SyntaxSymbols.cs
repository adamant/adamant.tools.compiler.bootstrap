using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics
{
    internal partial class SemanticAnalysis
    {
        public const string SyntaxSymbolAttribute = "SyntaxSymbol";

        public PackageSyntaxSymbol PackageSyntaxSymbol => SyntaxSymbol(PackageSyntax);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public PackageSyntaxSymbol SyntaxSymbol(PackageSyntax syntax)
        {
            return attributes.GetOrAdd(syntax, SyntaxSymbolAttribute, ComputeSyntaxSymbol);
        }

        private static PackageSyntaxSymbol ComputeSyntaxSymbol(PackageSyntax package)
        {
            var compilationUnits = package.CompilationUnits.ToList();
            var globalNamespace = new SyntaxSymbol(compilationUnits,
                ComputeSyntaxSymbol(compilationUnits.SelectMany(cu => cu.Children.OfType<DeclarationSyntax>())));
            return new PackageSyntaxSymbol(package, globalNamespace);
        }

        private static IEnumerable<SyntaxSymbol> ComputeSyntaxSymbol(IEnumerable<DeclarationSyntax> declarations)
        {
            return declarations.GroupBy(d => d.Name.Value).Select(g =>
            {
                var children = ComputeSyntaxSymbol(g.SelectMany(ChildrenWithSymbols));
                return new SyntaxSymbol(g.Key, g, children);
            });
        }

        private static IEnumerable<DeclarationSyntax> ChildrenWithSymbols(SyntaxBranchNode declaration)
        {
            switch (declaration)
            {
                case FunctionDeclarationSyntax function:
                    return function.Parameters;
                default:
                    return declaration.Children.OfType<DeclarationSyntax>();
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SyntaxSymbol SyntaxSymbol(SyntaxBranchNode syntax)
        {
            return attributes.GetOrAdd(syntax, SyntaxSymbolAttribute, ComputeSyntaxSymbol);
        }

        private SyntaxSymbol ComputeSyntaxSymbol(SyntaxBranchNode syntax)
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
