using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.IL
{
    public class PackageIL
    {
        [NotNull] public readonly string Name;

        public PackageIL([NotNull] string name)
        {
            Requires.NotNull(nameof(name), name);
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
