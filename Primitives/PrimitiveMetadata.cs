using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal abstract class PrimitiveMetadata : IMetadata
    {
        public Name FullName { get; }
        public MetadataSet ChildMetadata { get; }

        protected PrimitiveMetadata(Name fullName, MetadataSet childMetadata)
        {
            FullName = fullName;
            ChildMetadata = childMetadata;
        }
    }
}
