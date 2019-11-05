using System.Globalization;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class BooleanConstant : Constant
    {
        public bool Value { get; }
        public override ValueSemantics ValueSemantics => ValueSemantics.Copy;

        public BooleanConstant(bool value, TextSpan span)
            : base(DataType.Bool, span)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
