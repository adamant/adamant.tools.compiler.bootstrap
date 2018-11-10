using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    public class ParseContext
    {
        [NotNull] public readonly CodeFile File;
        [NotNull] public readonly Diagnostics Diagnostics;

        public ParseContext([NotNull] CodeFile file, [NotNull] Diagnostics diagnostics)
        {
            File = file.NotNull();
            Diagnostics = diagnostics.NotNull();
        }
    }
}
