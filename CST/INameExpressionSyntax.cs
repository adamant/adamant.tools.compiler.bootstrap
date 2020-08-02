using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// An expression that is a single unqualified name
    /// </summary>
    public partial interface INameExpressionSyntax
    {
        [DisallowNull] IBindingMetadata? ReferencedBinding { get; set; }
        FixedList<IMetadata> LookupInContainingScope();
        bool VariableIsLiveAfter { get; set; }
    }
}
