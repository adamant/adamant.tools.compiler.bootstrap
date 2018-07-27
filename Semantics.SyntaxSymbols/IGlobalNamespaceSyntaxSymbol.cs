using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public interface IGlobalNamespaceSyntaxSymbol : ISyntaxSymbol
    {
        new IReadOnlyList<CompilationUnitSyntax> Declarations { get; }
    }
}
