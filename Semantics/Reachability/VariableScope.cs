using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    public class VariableScope
    {
        private readonly VariableScope? containingScope;
        private readonly List<VariablePlace> variables = new List<VariablePlace>();

        public VariableScope(VariableScope? containingScope)
        {
            this.containingScope = containingScope;
        }

        public VariablePlace AddVariable(SimpleName name)
        {
            var variable = new VariablePlace(name);
            variables.Add(variable);
            return variable;
        }
    }
}
