using static Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph.Access;
using static Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph.Ownership;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal abstract class HeapPlace : Place
    {
        public Reference NewSharedReference()
        {
            return new Reference(this, None, ReadOnly);
        }

        public Reference NewIdentityReference()
        {
            return new Reference(this, None, Identity);
        }
    }
}
