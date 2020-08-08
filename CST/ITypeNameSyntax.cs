using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface ITypeNameSyntax
    {
        TypeSymbol ReferencedSymbol { get; set; }
        [DisallowNull] IMetadata? ReferencedMetadata { get; set; }
        FixedList<IMetadata> LookupMetadataInContainingScope();
    }
}
