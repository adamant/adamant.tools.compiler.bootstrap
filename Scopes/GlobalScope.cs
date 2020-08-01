using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    public class GlobalScope : LexicalScope
    {
        public GlobalScope(IEnumerable<IMetadata> metadata, IEnumerable<IMetadata> nestedMetadata)
            : base(new MetadataSet(metadata), new MetadataSet(nestedMetadata))
        {
        }

        public override FixedList<IMetadata> LookupMetadataInGlobalScope(Name name)
        {
            // Don't include nested scopes, it must be in the global scope because it is global qualified
            return LookupMetadata(name, includeNested: false);
        }
    }
}
