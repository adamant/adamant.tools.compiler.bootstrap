using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// An expression that is a single unqualified name
    /// </summary>
    public interface INameExpressionSyntax : IAssignableExpressionSyntax, IHasContainingScope
    {
        SimpleName Name { get; }
        [DisallowNull] IBindingSymbol? ReferencedSymbol { get; set; }
        FixedList<ISymbol> LookupInContainingScope();
        bool VariableIsLiveAfter { get; set; }
    }
}
