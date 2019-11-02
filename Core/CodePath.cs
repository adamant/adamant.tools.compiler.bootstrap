using System.IO;
using System.Threading.Tasks;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    /// A CodeReference to a file on disk referenced by its path.
    public class CodePath : CodeReference, ICodeFileSource
    {
        public string Path { get; }

        public CodePath(string path)
            : this(path, FixedList<string>.Empty)
        {
        }

        public CodePath(string path, FixedList<string> @namespace)
            : base(@namespace)
        {
            Requires.That(nameof(path), System.IO.Path.IsPathFullyQualified(path), "must be fully qualified");
            Path = path;
        }

        public override string ToString()
        {
            return Path;
        }

        public CodeFile Load()
        {
            var text = File.ReadAllText(Path, CodeFile.Encoding);
            return new CodeFile(this, new CodeText(text));
        }

        public async Task<CodeFile> LoadAsync()
        {
            var text = await File.ReadAllTextAsync(Path, CodeFile.Encoding).ConfigureAwait(false);
            return new CodeFile(this, new CodeText(text));
        }
    }
}
