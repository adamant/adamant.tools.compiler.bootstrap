namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    /// A CodeReference to a file on disk referenced by its path.
    public class CodePath : CodeReference
    {
        public readonly string Path;

        public CodePath(string path)
        {
            Path = path;
        }
    }
}
