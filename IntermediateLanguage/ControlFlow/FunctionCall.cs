using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class FunctionCall : Value
    {
        public readonly Name FunctionName;
        public readonly FunctionType Type;
        public readonly Operand Self;
        public FixedList<Operand> Arguments { get; }
        public int Arity => Arguments.Count;

        public FunctionCall(
            Name functionName,
            FunctionType type,
            Operand self,
            IEnumerable<Operand> arguments,
            TextSpan span)
            : base(span)
        {
            FunctionName = functionName;
            Type = type;
            Self = self;
            Arguments = arguments.ToFixedList();
        }

        public FunctionCall(Name functionName, FunctionType type, IEnumerable<Operand> arguments, TextSpan span)
            : this(functionName, type, null, arguments, span)
        {
        }

        public FunctionCall(
            TextSpan span,
            Name functionName,
            FunctionType type,
            params Operand[] arguments)
            : this(functionName, type, null, arguments, span)
        {
        }

        // Useful for debugging
        public override string ToString()
        {
            return $"call {FunctionName}({string.Join(", ", Arguments)});";
        }
    }
}
