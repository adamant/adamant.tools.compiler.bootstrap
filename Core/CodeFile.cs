using System.IO;
using System.Text;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    /// A CodeFile represents the combination of CodeText and CodePath
    public class CodeFile
    {
        /// Source code files are encoded with UTF-8 without a BOM. C# UTF-8 include
        /// the BOM by default. So we make our own Encoding object.
        [NotNull]
        public static readonly Encoding Encoding = new UTF8Encoding(false);

        [NotNull]
        public CodeReference Reference { get; }

        [NotNull]
        public CodeText Code { get; }

        public CodeFile([NotNull] CodeReference reference, [NotNull] CodeText text)
        {
            Code = text;
            Reference = reference;
        }

        [NotNull]
        public static CodeFile Load([NotNull] string path)
        {
            return Load(path, FixedList<string>.Empty);
        }

        [NotNull]
        public static CodeFile Load([NotNull] string path, [NotNull] FixedList<string> @namespace)
        {
            var fullPath = Path.GetFullPath(path);
            return new CodeFile(new CodePath(fullPath, @namespace), new CodeText(File.ReadAllText(fullPath, Encoding)));
        }
    }
}
