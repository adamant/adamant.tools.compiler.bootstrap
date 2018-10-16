using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Declarations
{
    public class Namespace
    {
        [NotNull] public Name Name { get; }

        public Namespace([NotNull] Name name)
        {
            Name = name;
        }
    }
}
