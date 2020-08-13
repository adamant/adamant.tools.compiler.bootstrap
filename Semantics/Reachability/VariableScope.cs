using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    /// <summary>
    /// A lexical scope of variable places used to track when variables come into
    /// and go out of scope.
    /// </summary>
    internal class VariableScope
    {
        public VariableScope? ContainingScope { get; }
        private readonly HashSet<BindingSymbol> variables = new HashSet<BindingSymbol>();
        public IReadOnlyCollection<BindingSymbol> Variables => variables;

        public VariableScope(VariableScope? containingScope = null)
        {
            ContainingScope = containingScope;
        }

        /// <summary>
        /// Declare a variable in the current scope
        /// </summary>
        public void VariableDeclared(BindingSymbol symbol)
        {
            variables.Add(symbol);
        }
    }
}
