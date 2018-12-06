namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public class ParseContext
    {
        public readonly CodeFile File;
        public readonly Diagnostics Diagnostics;

        public ParseContext(CodeFile file, Diagnostics diagnostics)
        {
            File = file;
            Diagnostics = diagnostics;
        }
    }
}
