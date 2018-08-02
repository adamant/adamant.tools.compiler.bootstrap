using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    /// <summary>
    /// This is an akward case because the syntax symbols have many declarations.
    /// To handle this, the whole tree is built for the package from the root.
    /// If symbols are needed attached to other nodes, this will be done by
    /// defining them as a lookup from the parent symbol. However, it isn't clear
    /// that will even be needed because symbols are used for name lookup which
    /// I think will always happen from the package root.
    /// </summary>
    public class SyntaxSymbolAttribute : SemanticAttribute
    {
        public const string Key = "SyntaxSymbol";
        public override string AttributeKey => Key;

        public SyntaxSymbolAttribute(SemanticAttributes attributes)
            : base(attributes)
        {
        }

        public SyntaxSymbol Package => Get(Attributes.Package);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public SyntaxSymbol Get(PackageSyntax syntax)
        {
            return Attributes.GetOrAdd(syntax, Key, Compute);
        }

        private SyntaxSymbol Compute(PackageSyntax package)
        {
            var compilationUnits = package.CompilationUnits.ToList();
            var globalNamespace = new SyntaxSymbol(compilationUnits,
                    Compute(compilationUnits.SelectMany(cu => cu.Children.OfType<DeclarationSyntax>())));
            return new SyntaxSymbol(package, globalNamespace.Yield());
        }

        private IEnumerable<SyntaxSymbol> Compute(IEnumerable<DeclarationSyntax> declarations)
        {
            return declarations.GroupBy(d => d.Name.Value).Select(g =>
            {
                var children = Compute(g.SelectMany(d => d.Children.OfType<DeclarationSyntax>()));
                return new SyntaxSymbol(g.Key, g, children);
            });
        }
    }
}
