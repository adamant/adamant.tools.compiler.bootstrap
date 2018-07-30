using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.SyntaxSymbols
{
    public interface IPackageSyntaxSymbol : ISyntaxSymbol
    {
        IGlobalNamespaceSyntaxSymbol GlobalNamespace { get; }
        PackageSyntax Declaration { get; }
        new IReadOnlyList<PackageSyntax> Declarations { get; }
        IDeclarationSyntaxSymbol Lookup(VariableName variableName);
    }
}
