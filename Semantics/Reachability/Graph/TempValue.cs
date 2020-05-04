using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class TempValue : RootPlace
    {
        public ReferenceType ReferenceType { get; }

        public TempValue(ReferenceType referenceType)
        {
            ReferenceType = referenceType;
        }
    }
}
