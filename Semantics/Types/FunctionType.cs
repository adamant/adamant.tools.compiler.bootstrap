using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    // A function type is the type of a declared function, method or constructor.
    // A function type may be generic and have generic parameters
    public class FunctionType : DataType
    {
        public readonly IReadOnlyList<DataType> ParameterTypes;
        public readonly DataType ReturnType;

        public FunctionType(IEnumerable<DataType> parameterTypes, DataType returnType)
        {
            ParameterTypes = parameterTypes.ToList().AsReadOnly();
            ReturnType = returnType;
        }

        public override string ToString()
        {
            return $"({string.Join(',', ParameterTypes)}) -> {ReturnType}";
        }
    }
}
