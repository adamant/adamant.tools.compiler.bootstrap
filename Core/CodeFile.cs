using System.IO;
using System.Text;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    /// A CodeFile represents the combination of CodeText and CodePath
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

        public static CodeFile Load(string path)
        {
            var fullPath = Path.GetFullPath(path);
            return new CodeFile(new CodeText(File.ReadAllText(fullPath, Encoding)), new CodePath(fullPath));
        }
    }
}
