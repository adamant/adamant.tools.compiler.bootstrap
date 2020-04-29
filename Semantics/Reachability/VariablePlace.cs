using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    public class VariablePlace : Place
    {
        public SimpleName Name { get; }

        public VariablePlace(SimpleName name)
        {
            Name = name;
        }
    }
}
