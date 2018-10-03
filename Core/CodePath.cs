using System.IO;
using System.Threading.Tasks;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    /// A CodeReference to a file on disk referenced by its path.
    public class CodePath : CodeReference, ICodeFileSource
    {
        public readonly string Path;

        public CodePath(string path)
        {
            Requires.That(nameof(path), System.IO.Path.IsPathFullyQualified(path));
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
            var text = await File.ReadAllTextAsync(Path, CodeFile.Encoding);
            return new CodeFile(this, new CodeText(text));
        }
    }
}
