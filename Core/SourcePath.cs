namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    /// A SourceReference to a file on disk referenced by its path.
    public class SourcePath : SourceReference
    {
        public readonly string Path;

        public SourcePath(string path)
        {
            Path = path;
        }
    }
}
