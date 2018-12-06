using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.ControlFlow
{
    public class Utf8BytesConstant : Constant
    {
        public static readonly Encoding Encoding = new UTF8Encoding(false);

        public readonly string Value;

        public Utf8BytesConstant(string value)
            : base(DataType.BytePointer)
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
