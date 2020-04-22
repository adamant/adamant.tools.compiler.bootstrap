using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.IntermediateLanguage.CFG
{
    public static class StringConstant
    {
        public static readonly Encoding Encoding = new UTF8Encoding(false);

        public static int GetByteLength(string value)
        {
            return Encoding.GetByteCount(value);
        }
    }
}
