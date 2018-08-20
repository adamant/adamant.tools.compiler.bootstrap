using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.LValues;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements
{
    public class CallStatement : Statement
    {
        public readonly LValue LValue;
        public readonly string FunctionName;
        public IReadOnlyList<LValue> Arguments { get; }
        private readonly List<LValue> arguments = new List<LValue>();

        public CallStatement(LValue lvalue, string functionName)
        {
            LValue = lvalue;
            FunctionName = functionName;
            Arguments = arguments.AsReadOnly();
        }

        internal override void ToString(AsmBuilder builder)
        {
            builder.AppendLine($"{LValue} = call {FunctionName}({string.Join(',', Arguments)})");
        }
    }
}
