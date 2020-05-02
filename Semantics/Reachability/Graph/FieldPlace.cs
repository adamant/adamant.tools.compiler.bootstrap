using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class FieldPlace : AssignablePlace
    {
        public new FieldPlaceIdentifier Identifier { get; }

        public FieldPlace(FieldPlaceIdentifier identifier, ReferenceType type)
            : base(identifier, type)
        {
            Identifier = identifier;
        }
    }
}
