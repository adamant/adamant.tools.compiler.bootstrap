using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    public sealed class DefaultConstructor : IFunctionMetadata
    {
        public MaybeQualifiedName FullName { get; }
        public DataType ConstructedType { get; }

        MetadataSet IParentMetadata.ChildMetadata => MetadataSet.Empty;

        IEnumerable<IBindingMetadata> IFunctionMetadata.Parameters => Enumerable.Empty<IBindingMetadata>();
        DataType IFunctionMetadata.ReturnDataType => ConstructedType;

        public DefaultConstructor(ObjectType constructedType)
        {
            ConstructedType = constructedType;
            FullName = constructedType.ContainingNamespace.ToRootName().Qualify(constructedType.Name.ToSimpleName()).Qualify(SpecialNames.New);
        }
    }
}
