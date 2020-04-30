using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Scopes
{
    internal abstract class VariableScope
    {
        private readonly PlaceIdentifierList identifiers;
        private readonly HashSet<VariablePlaceIdentifier> variables = new HashSet<VariablePlaceIdentifier>();

        protected VariableScope(PlaceIdentifierList identifiers)
        {
            this.identifiers = identifiers;
        }

        public abstract CallerVariableScope CallerScope { get; }

        /// <summary>
        /// Declare a variable in the current scope
        /// </summary>
        public VariablePlaceIdentifier VariableDeclared(SimpleName name)
        {
            var variable = identifiers.VariableIdentifierFor(name);
            variables.Add(variable);
            return variable;
        }
    }
}
