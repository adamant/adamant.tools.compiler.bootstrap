using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.LValues;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements
{
    public class CallStatement : Statement
    {
        public readonly LValue ReturnInto;
        public readonly string FunctionName;
        public IReadOnlyList<LValue> Arguments { get; }
        private readonly List<LValue> arguments = new List<LValue>();

        public CallStatement(LValue returnInto, string functionName)
        {
            ReturnInto = returnInto;
            FunctionName = functionName;
            Arguments = arguments.AsReadOnly();
        }
    }
}
