using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class FunctionCall : Value
    {
        public readonly Name FunctionName;
        public readonly Operand Self;
        public FixedList<Operand> Arguments { get; }
        public int Arity => Arguments.Count;

        public FunctionCall(
            Name functionName,
            Operand self,
            IEnumerable<Operand> arguments,
            TextSpan span)
            : base(span)
        {
            FunctionName = functionName;
            Self = self;
            Arguments = arguments.ToFixedList();
        }

        public FunctionCall(Name functionName, IEnumerable<Operand> arguments, TextSpan span)
            : this(functionName, null, arguments, span)
        {
        }

        public FunctionCall(
            TextSpan span,
            Name functionName,
            params Operand[] arguments)
            : this(functionName, null, arguments, span)
        {
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"call {FunctionName}({string.Join(", ", Arguments)});";
        }
    }
}
