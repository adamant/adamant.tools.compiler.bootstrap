namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public class ParseContext
    {
        public CodeFile File { get; }
        public Diagnostics Diagnostics { get; }

        public ParseContext(CodeFile file, Diagnostics diagnostics)
        {
            File = file;
            Diagnostics = diagnostics;
        }
    }
}
