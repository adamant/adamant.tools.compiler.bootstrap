using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    /// <summary>
    /// A lexical scope of variable places used to track when variables come into
    /// and go out of scope.
    /// </summary>
    internal class VariableScope
    {
        public VariableScope? ContainingScope { get; }
        private readonly HashSet<IBindingMetadata> variables = new HashSet<IBindingMetadata>();
        public IReadOnlyCollection<IBindingMetadata> Variables => variables;

        public VariableScope(VariableScope? containingScope = null)
        {
            ContainingScope = containingScope;
        }

        /// <summary>
        /// Declare a variable in the current scope
        /// </summary>
        public void VariableDeclared(IBindingMetadata symbol)
        {
            variables.Add(symbol);
        }
    }
}
