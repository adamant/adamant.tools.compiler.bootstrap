using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Statements
{
    public class CallStatement : SimpleStatement
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
    }
}
