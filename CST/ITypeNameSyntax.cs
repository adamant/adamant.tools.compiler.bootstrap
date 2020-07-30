using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface ITypeNameSyntax : ITypeSyntax, IHasContainingScope
    {
        [DisallowNull] IMetadata? ReferencedType { get; set; }
        FixedList<IMetadata> LookupInContainingScope();
    }
}
