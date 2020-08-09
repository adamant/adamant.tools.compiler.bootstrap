using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public partial interface ITypeNameSyntax
    {
        IPromise<TypeSymbol?> ReferencedSymbol { get; set; }
        IEnumerable<IPromise<Symbol>> LookupInContainingScope();
        [DisallowNull] IMetadata? ReferencedMetadata { get; set; }
        FixedList<IMetadata> LookupMetadataInContainingScope();
    }
}
