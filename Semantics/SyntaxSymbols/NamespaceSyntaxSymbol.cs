using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public class NamespaceSyntaxSymbol : ISyntaxSymbol
    {
        public string Name { get; }
        private readonly HashSet<ISyntaxSymbol> children = new HashSet<ISyntaxSymbol>();
        public IEnumerable<ISyntaxSymbol> Children => children;

        //private readonly IList<NamespaceSyntax> declarations;
        //public IEnumerable<NamespaceSyntax> Declarations => declarations;
        //IEnumerable<SyntaxNode> ISyntaxSymbol.Declarations => declarations;
        IEnumerable<SyntaxNode> ISyntaxSymbol.Declarations => throw new System.NotImplementedException();

        public NamespaceSyntaxSymbol(string name)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
        }

        public void AddChild(ISyntaxSymbol child)
        {
            children.Add(child);
        }
    }
}
