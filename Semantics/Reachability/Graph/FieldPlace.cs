using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class FieldPlace : AssignablePlace
    {
        public new FieldPlaceIdentifier Identifier { get; }

        public FieldPlace(FieldPlaceIdentifier identifier)
            : base(identifier)
        {
            Identifier = identifier;
        }
    }
}
