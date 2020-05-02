using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Scopes
{
    /// <summary>
    /// A lexical scope of variable places used to track when variables come into
    /// and go out of scope.
    /// </summary>
    internal class LexicalVariableScope : VariableScope
    {
        private readonly VariableScope containingScope;
        private readonly HashSet<IBindingSymbol> variables = new HashSet<IBindingSymbol>();

        public LexicalVariableScope(VariableScope containingScope)
        {
            this.containingScope = containingScope;
        }

        public override CallerVariableScope CallerScope => containingScope.CallerScope;

        /// <summary>
        /// Declare a variable in the current scope
        /// </summary>
        public void VariableDeclared(IBindingSymbol symbol)
        {
            variables.Add(symbol);
        }
    }
}
