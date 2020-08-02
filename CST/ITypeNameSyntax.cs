using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface ITypeNameSyntax
    {
        [DisallowNull] IMetadata? ReferencedType { get; set; }
        FixedList<IMetadata> LookupInContainingScope();
    }
}
