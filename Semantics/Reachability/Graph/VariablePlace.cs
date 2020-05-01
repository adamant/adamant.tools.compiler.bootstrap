using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class VariablePlace : AssignablePlace
    {
        public new VariablePlaceIdentifier Identifier { get; }

        public VariablePlace(VariablePlaceIdentifier identifier)
            : base(identifier)
        {
            Identifier = identifier;
        }
    }
}
