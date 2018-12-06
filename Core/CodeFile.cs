using System.IO;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    /// A CodeFile represents the combination of CodeText and CodePath
    public class CodeFile
    {
        /// Source code files are encoded with UTF-8 without a BOM. C# UTF-8 include
        /// the BOM by default. So we make our own Encoding object.

        public static readonly Encoding Encoding = new UTF8Encoding(false);

        public CodeReference Reference { get; }

        public CodeText Code { get; }

        public CodeFile(CodeReference reference, CodeText text)
        {
            Code = text;
            Reference = reference;
        }

        public static CodeFile Load(string path)
        {
            return Load(path, FixedList<string>.Empty);
        }

        public static CodeFile Load(string path, FixedList<string> @namespace)
        {
            var fullPath = Path.GetFullPath(path);
            return new CodeFile(new CodePath(fullPath, @namespace), new CodeText(File.ReadAllText(fullPath, Encoding)));
        }
    }
}
