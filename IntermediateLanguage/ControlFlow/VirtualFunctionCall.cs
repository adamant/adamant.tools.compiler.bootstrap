using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class VirtualFunctionCall : Value
    {
        public SimpleName FunctionName { get; }
        public IOperand Self { get; }
        public FixedList<IOperand> Arguments { get; }

        public VirtualFunctionCall(
            TextSpan span,
            SimpleName functionName,
            IOperand self,
            IEnumerable<IOperand> arguments)
            : base(span)
        {
            FunctionName = functionName;
            Self = self;
            Arguments = arguments.ToFixedList();
        }

        public VirtualFunctionCall(
            TextSpan span,
            SimpleName functionName,
            IOperand self,
            params IOperand[] arguments)
            : this(span, functionName, self, arguments as IEnumerable<IOperand>)
        {
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"call virtual ({Self}).{FunctionName}({string.Join(", ", Arguments)})";
        }
    }
}
