using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public interface ISyntaxSymbol
    {
        string Name { get; }
        IEnumerable<ISyntaxSymbol> Children { get; }
        IEnumerable<SyntaxNode> Declarations { get; }
    }
}
