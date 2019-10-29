using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class FunctionCall : Value
    {
        public Name FunctionName { get; }
        public IOperand? Self { get; }
        public FixedList<IOperand> Arguments { get; }
        public int Arity => Arguments.Count;

        public FunctionCall(
            Name functionName,
            IOperand? self,
            IEnumerable<IOperand> arguments,
            TextSpan span)
            : base(span)
        {
            FunctionName = functionName;
            Self = self;
            Arguments = arguments.ToFixedList();
        }

        public FunctionCall(Name functionName, IEnumerable<IOperand> arguments, TextSpan span)
            : this(functionName, null, arguments, span)
        {
        }

        public FunctionCall(
            TextSpan span,
            Name functionName,
            params IOperand[] arguments)
            : this(functionName, null, arguments, span)
        {
        }

        // Useful for debugging
        public override string ToString()
        {
            var arguments = string.Join(", ", Arguments);
            return Self != null
                // For method invocation, we need to know what self
                ? $"call method {FunctionName}({string.Join(", ", Self, arguments)})"
                : $"call fn {FunctionName}({arguments})";
        }
    }
}
