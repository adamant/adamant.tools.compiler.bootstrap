using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    internal class Argument
    {
        public TempValue? Value { get; }
        public IExpression Expression { get; }
        public DataType ParameterDataType { get; }

        public Argument(IExpression expression, DataType parameterDataType, TempValue? value)
        {
            Value = value;
            Expression = expression;
            ParameterDataType = parameterDataType;
        }
    }
}
