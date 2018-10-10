using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Declarations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public interface IDeclarationSyntaxSymbol : ISyntaxSymbol
    {
        new IEnumerable<DeclarationSyntax> Declarations { get; }
    }
}
