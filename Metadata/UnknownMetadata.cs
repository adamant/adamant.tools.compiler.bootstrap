using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    public class UnknownMetadata : ITypeMetadata, IFunctionMetadata, IBindingMetadata
    {
        #region Singleton
        public static readonly UnknownMetadata Instance = new UnknownMetadata();

        private UnknownMetadata() { }
        #endregion

        // We don't know what this is, so it might be mutable (fewer errors that way)
        public bool IsMutableBinding => true;
        public MaybeQualifiedName FullName => SpecialNames.Unknown;
        public DataType DataType => DataType.Unknown;
        public DataType DeclaresDataType => DataType.Unknown;
        public MetadataSet ChildMetadata => MetadataSet.Empty;

        public IEnumerable<IBindingMetadata> Parameters => Enumerable.Empty<IBindingMetadata>();

        public DataType ReturnDataType => DataType.Unknown;
    }
}
