using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public class DeclarationSyntaxSymbol : ISyntaxSymbol
    {
        public string Name { get; }
        private readonly IList<ISyntaxSymbol> children;
        public IEnumerable<ISyntaxSymbol> Children => children;

        private readonly IList<DeclarationSyntax> declarations;
        public IEnumerable<DeclarationSyntax> Declarations => declarations;
        IEnumerable<SyntaxNode> ISyntaxSymbol.Declarations => declarations;

        /// For variables and parameters, the type of the value they represent.
        /// For class/struct/enum symbols, the type being declared
        public readonly Type type;
    }
}
