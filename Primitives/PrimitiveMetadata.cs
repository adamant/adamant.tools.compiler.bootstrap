using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal abstract class PrimitiveMetadata : IMetadata
    {
        public MaybeQualifiedName FullName { get; }
        public MetadataSet ChildMetadata { get; }

        protected PrimitiveMetadata(MaybeQualifiedName fullName, MetadataSet childMetadata)
        {
            FullName = fullName;
            ChildMetadata = childMetadata;
        }
    }
}
