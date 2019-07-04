using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public abstract class Constant : Value, IOperand
    {
        public readonly DataType Type;

        protected Constant(DataType type, TextSpan span)
            : base(span)
        {
            Type = type;
        }
    }
}
