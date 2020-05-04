using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Scopes
{
    /// <summary>
    /// A lexical scope of variable places used to track when variables come into
    /// and go out of scope.
    /// </summary>
    internal class VariableScope
    {
        public VariableScope? ContainingScope { get; }
        private readonly HashSet<IBindingSymbol> variables = new HashSet<IBindingSymbol>();
        public IReadOnlyCollection<IBindingSymbol> Variables => variables;

        public VariableScope(VariableScope? containingScope = null)
        {
            ContainingScope = containingScope;
        }

        /// <summary>
        /// Declare a variable in the current scope
        /// </summary>
        public void VariableDeclared(IBindingSymbol symbol)
        {
            variables.Add(symbol);
        }
    }
}
