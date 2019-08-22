using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class NoneConstant : Constant
    {
        public new readonly OptionalType Type;

        public NoneConstant(OptionalType type, TextSpan span)
            : base(type, span)
        {
            Type = type;
        }

        public override string ToString()
        {
            return "none";
        }
    }
}
