using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class StringConstant : Constant
    {
        public static readonly Encoding Encoding = new UTF8Encoding(false);

        public readonly string Value;

        public StringConstant(string value, TextSpan span)
            : base(DataType.StringConstant, span)
        {
            Value = value;
        }

        // Useful for debugging
        public override string ToString()
        {
            return "\"" + Value.Escape() + "\"";
        }
    }
}
