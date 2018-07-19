using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    /// <summary>
    /// A CodeFile represents the combination of CodeText and CodePath
    /// </summary>
    public class CodeFile
    {
        /// Source code files are encoded with UTF-8 without a BOM. C# UTF-8 include
        /// the BOM by default. So we make our own Encoding object.
        public static readonly Encoding Encoding = new UTF8Encoding(false);

        public CodeText Code { get; }
        public CodeReference Reference { get; }

        public CodeFile(CodeText text, CodeReference reference)
        {
            Code = text;
            Reference = reference;
        }
    }
}
