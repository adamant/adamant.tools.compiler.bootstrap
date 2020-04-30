using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    internal abstract class PlaceScope
    {
        private readonly HashSet<VariablePlace> variables = new HashSet<VariablePlace>();


        protected readonly PlaceList Places;

        protected PlaceScope(PlaceList places)
        {
            Places = places;
        }

        public abstract CallerPlaceScope CallerScope { get; }

        /// <summary>
        /// Declare a variable in the current scope
        /// </summary>
        public VariablePlace VariableDeclared(SimpleName name)
        {
            var variable = Places.GetOrAddVariable(name);
            variables.Add(variable);
            return variable;
        }
    }
}
