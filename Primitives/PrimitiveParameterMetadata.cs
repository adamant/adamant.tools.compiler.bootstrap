using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class PrimitiveParameterMetadata : PrimitiveMetadata, IBindingMetadata
    {
        public SimpleName Name => FullName.UnqualifiedName;

        public DataType DataType { get; }

        public bool IsMutableBinding => false;

        public PrimitiveParameterMetadata(MaybeQualifiedName fullName, DataType type)
            : base(fullName, MetadataSet.Empty)
        {
            DataType = type;
        }
    }
}