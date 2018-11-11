using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Core
{
    /// Some kind of reference to where the source code came from. For example,
    /// this might be a path on disk or a network URL or a git hash or what
    /// template file the code was generated from.
    public abstract class CodeReference
    {
        [NotNull] public readonly FixedList<string> Namespace;

        protected CodeReference([NotNull] FixedList<string> @namespace)
        {
            Namespace = @namespace;
        }
    }
}
