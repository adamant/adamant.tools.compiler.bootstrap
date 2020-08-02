using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// An expression that is a single unqualified name
    /// </summary>
    public partial interface INameExpressionSyntax : IAssignableExpressionSyntax, IHasContainingScope
    {
        SimpleName Name { get; }
        [DisallowNull] IBindingMetadata? ReferencedBinding { get; set; }
        FixedList<IMetadata> LookupInContainingScope();
        bool VariableIsLiveAfter { get; set; }
    }
}
