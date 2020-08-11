using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface ISelfExpressionSyntax
    {
        [DisallowNull] IBindingMetadata? ReferencedBinding { get; set; }
        FixedList<IMetadata> LookupMetadataInContainingScope();
    }
}
