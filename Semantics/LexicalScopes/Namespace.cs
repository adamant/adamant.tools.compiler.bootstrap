using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.LexicalScopes
{
    internal class Namespace : INamespaceMetadata
    {
        public Name FullName { get; }
        public MetadataSet ChildMetadata { get; }
        public MetadataSet NestedSymbols { get; }
        public Namespace(Name fullName, IEnumerable<IMetadata> childSymbols, IEnumerable<IMetadata> nestedSymbols)
        {
            FullName = fullName;
            ChildMetadata = new MetadataSet(childSymbols);
            NestedSymbols = new MetadataSet(nestedSymbols);
        }
    }
}
