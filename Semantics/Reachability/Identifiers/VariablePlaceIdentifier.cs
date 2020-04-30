using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers
{
    public class VariablePlaceIdentifier : PlaceIdentifier
    {
        public SimpleName VariableName { get; }

        public VariablePlaceIdentifier(SimpleName variableName)
        {
            VariableName = variableName;
        }
    }
}
