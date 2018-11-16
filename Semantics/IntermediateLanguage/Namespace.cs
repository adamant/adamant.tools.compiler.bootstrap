using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.IntermediateLanguage
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
