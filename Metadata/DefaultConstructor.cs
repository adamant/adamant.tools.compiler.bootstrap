using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    public sealed class DefaultConstructor : IFunctionMetadata
    {
        public ConstructorSymbol Symbol { get; }
        public MaybeQualifiedName FullName { get; }
        public DataType ConstructedType { get; }

        MetadataSet IParentMetadata.ChildMetadata => MetadataSet.Empty;
        public SelfParameterSymbol ImplicitSelfParameterSymbol { get; }
        IEnumerable<IBindingMetadata> IFunctionMetadata.Parameters => Enumerable.Empty<IBindingMetadata>();
        DataType IFunctionMetadata.ReturnDataType => ConstructedType;

        public DefaultConstructor(ConstructorSymbol symbol, SelfParameterSymbol selfParameterSymbol)
        {
            Symbol = symbol;
            var constructedType = symbol.ContainingSymbol.DeclaresDataType;
            ConstructedType = constructedType;
            ImplicitSelfParameterSymbol = selfParameterSymbol;
            FullName = constructedType.ContainingNamespace.ToRootName().Qualify(constructedType.Name.ToSimpleName()).Qualify(SpecialNames.New);
        }
    }
}
