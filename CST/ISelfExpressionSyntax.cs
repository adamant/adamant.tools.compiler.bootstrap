using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface ISelfExpressionSyntax : IExpressionSyntax, IHasContainingScope
    {
        bool IsImplicit { get; }
        [DisallowNull] IBindingSymbol? ReferencedSymbol { get; set; }
        FixedList<ISymbol> LookupInContainingScope();
    }
}
