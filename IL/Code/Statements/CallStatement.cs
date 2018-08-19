using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.IL.Code.LValues;

namespace Adamant.Tools.Compiler.Bootstrap.IL.Code.Statements
{
    public class CallStatement : Statement
    {
        public readonly LValue ResultInto;
        public readonly string FunctionName;
        public IReadOnlyList<LValue> Arguments { get; }
        private readonly List<LValue> arguments = new List<LValue>();

        public CallStatement(LValue resultInto, string functionName)
        {
            ResultInto = resultInto;
            FunctionName = functionName;
            Arguments = arguments.AsReadOnly();
        }
    }
}
