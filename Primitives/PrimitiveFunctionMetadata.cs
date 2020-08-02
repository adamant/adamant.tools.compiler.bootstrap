using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Primitives
{
    internal class PrimitiveFunctionMetadata : PrimitiveMetadata, IFunctionMetadata
    {
        public FixedList<PrimitiveParameterMetadata> Parameters { get; }

        IEnumerable<IBindingMetadata> IFunctionMetadata.Parameters => Parameters;

        public DataType ReturnDataType { get; }

        protected PrimitiveFunctionMetadata(Name fullName, FixedList<PrimitiveParameterMetadata> parameters, DataType returnType)
            : base(fullName, new MetadataSet(parameters))
        {
            Parameters = parameters;
            ReturnDataType = returnType;
        }

        public static PrimitiveFunctionMetadata New(
            Name fullName,
            params (string name, DataType type)[] parameters)
        {
            return new PrimitiveFunctionMetadata(fullName, ConvertParameters(fullName, parameters), DataType.Void);
        }

        public static PrimitiveFunctionMetadata New(
            Name fullName,
            DataType returnType,
            params (string name, DataType type)[] parameters)
        {
            return new PrimitiveFunctionMetadata(fullName, ConvertParameters(fullName, parameters), returnType);
        }

        protected static FixedList<PrimitiveParameterMetadata> ConvertParameters(
            Name fullName,
            params (string name, DataType type)[] parameters)
        {
            return parameters.Select(p => new PrimitiveParameterMetadata(fullName.Qualify(p.name), p.type))
                .ToFixedList();
        }
    }
}
