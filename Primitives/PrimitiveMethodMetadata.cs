using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class PrimitiveMethodMetadata : PrimitiveFunctionMetadata, IMethodMetadata
    {
        public IBindingMetadata SelfParameterMetadata { get; }

        private PrimitiveMethodMetadata(
            MaybeQualifiedName fullName,
            PrimitiveParameterMetadata selfParameter,
            FixedList<PrimitiveParameterMetadata> parameters,
            DataType returnType)
            : base(fullName, parameters, returnType)
        {
            SelfParameterMetadata = selfParameter;
        }

        public static PrimitiveMethodMetadata New(
            MaybeQualifiedName fullName,
            DataType returnType,
            DataType selfType,
            params (string name, DataType type)[] parameters)
        {
            var selfParameter = new PrimitiveParameterMetadata(fullName.Qualify(SpecialNames.Self), selfType);
            return new PrimitiveMethodMetadata(fullName, selfParameter, ConvertParameters(fullName, parameters), returnType);
        }
    }
}
