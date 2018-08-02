using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public class SyntaxSymbolBuilder
    {
        public SyntaxAnnotation<ISyntaxSymbol> Build(PackageSyntax package)
        {
            var symbols = new SyntaxAnnotation<ISyntaxSymbol>();
            var packageSymbol = new PackageSyntaxSymbol(package);
            symbols.Add(package, packageSymbol);
            foreach (var compilationUnit in package.SyntaxTrees.Select(t => t.Root))
                Build(packageSymbol.GlobalNamespace, compilationUnit, symbols);

            return symbols;
        }

        private static void Build(
            GlobalNamespaceSyntaxSymbol globalNamespace,
            CompilationUnitSyntax compilationUnit,
            SyntaxAnnotation<ISyntaxSymbol> symbols)
        {
            // Basically, every compilation unit is a declaration of the global namespace
            globalNamespace.AddDeclaration(compilationUnit);
            symbols.Add(compilationUnit, globalNamespace);
            foreach (var declaration in compilationUnit.Declarations)
                Build(globalNamespace, declaration, symbols);
        }

        private static void Build(GlobalNamespaceSyntaxSymbol globalNamespace, DeclarationSyntax declaration, SyntaxAnnotation<ISyntaxSymbol> symbols)
        {
            Match.On(declaration).With(m => m
                .Is<FunctionDeclarationSyntax>(f =>
                {
                    var symbol = new DeclarationSyntaxSymbol(f.Name.Value, f);
                    globalNamespace.AddChild((symbol));
                    symbols.Add(f, symbol);
                    foreach (var parameter in f.ParameterList.Parameters)
                        Build(symbol, parameter, symbols);
                }));
        }

        private static void Build(DeclarationSyntaxSymbol functionSymbol, ParameterSyntax parameter, SyntaxAnnotation<ISyntaxSymbol> symbols)
        {
            // TODO eventually we need to pay attention to redeclarations
            var symbol = new DeclarationSyntaxSymbol(parameter.Name.Value, parameter);
            functionSymbol.AddChild(symbol);
            symbols.Add(parameter, symbol);
        }
    }
}
