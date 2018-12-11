using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class VirtualFunctionCall : Value
    {
        public readonly SimpleName FunctionName;
        public readonly Operand Self;
        public readonly FixedList<Operand> Arguments;

        public VirtualFunctionCall(SimpleName functionName, Operand self, IEnumerable<Operand> arguments)
        {
            FunctionName = functionName;
            Self = self;
            Arguments = arguments.ToFixedList();
        }

        public VirtualFunctionCall(
            SimpleName functionName,
            Operand self,
            params Operand[] arguments)
            : this(functionName, self, arguments as IEnumerable<Operand>)
        {
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"virt_call {FunctionName}({string.Join(", ", Arguments)});";
        }
    }
}
