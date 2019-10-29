using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    [Closed(
        typeof(NoneConstant),
        typeof(StringConstant),
        typeof(IntegerConstant),
        typeof(BooleanConstant))]
    public abstract class Constant : Value, IOperand
    {
        public DataType Type { get; }

        protected Constant(DataType type, TextSpan span)
            : base(span)
        {
            Type = type;
        }
    }
}
