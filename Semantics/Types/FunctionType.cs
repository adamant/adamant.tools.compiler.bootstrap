using System.Collections.Generic;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Types
{
    public class FunctionType : DataType
    {
        public readonly IReadOnlyList<DataType> ParameterTypes;
        public readonly DataType ReturnType;

        public FunctionType(IEnumerable<DataType> parameterTypes, DataType returnType)
        {
            ParameterTypes = parameterTypes.ToList().AsReadOnly();
            ReturnType = returnType;
        }
    }
}
