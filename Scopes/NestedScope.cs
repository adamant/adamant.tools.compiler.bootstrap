using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    public class NestedScope : LexicalScope
    {
        public LexicalScope ContainingScope { get; }

        public NestedScope(
            LexicalScope containingScope,
            MetadataSet metadata,
            MetadataSet? nestedMetadata = null)
            : base(metadata, nestedMetadata ?? MetadataSet.Empty)
        {
            ContainingScope = containingScope;
        }

        public NestedScope(LexicalScope containingScope, IEnumerable<IMetadata> symbols)
            : this(containingScope, new MetadataSet(symbols), MetadataSet.Empty)
        { }

        public NestedScope(LexicalScope containingScope, IMetadata symbol)
            : this(containingScope, new MetadataSet(symbol.Yield()), MetadataSet.Empty)
        { }

        public override FixedList<IMetadata> LookupMetadataInGlobalScope(Name name)
        {
            return ContainingScope.LookupMetadataInGlobalScope(name);
        }

        public override FixedList<IMetadata> LookupMetadata(SimpleName name, bool includeNested = true)
        {
            var symbols = base.LookupMetadata(name, includeNested);
            return symbols.Any() ? symbols : ContainingScope.LookupMetadata(name, includeNested);
        }
    }
}
