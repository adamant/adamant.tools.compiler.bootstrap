using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface INameExpressionSyntax : IExpressionSyntax, IHasContainingScope
    {
        SimpleName Name { get; }
        [DisallowNull] ISymbol? ReferencedSymbol { get; set; }
        FixedList<ISymbol> LookupInContainingScope();
    }
}
