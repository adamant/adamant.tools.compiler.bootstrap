using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Scopes
{
    internal class CallerVariableScope : VariableScope
    {
        public CallerVariableScope(PlaceIdentifierList identifiers)
            : base(identifiers) { }

        public override CallerVariableScope CallerScope => this;
    }
}
