using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.NameBinding
{
    internal class Namespace
    {
        public Name Name { get; }

        public Namespace(Name name)
        {
            Name = name;
        }
    }
}
