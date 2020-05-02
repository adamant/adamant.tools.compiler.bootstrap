using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Scopes
{
    internal class CallerVariableScope : VariableScope
    {
        private readonly Dictionary<IBindingSymbol, VariablePlace> callerVariables = new Dictionary<IBindingSymbol, VariablePlace>();

        public override CallerVariableScope CallerScope => this;

        public VariablePlace CallerVariable(IParameterSyntax parameter)
        {
            if (!callerVariables.TryGetValue(parameter, out var place))
            {
                place = new VariablePlace(parameter);
                callerVariables.Add(place.Symbol, place);
            }

            return place;
        }
    }
}
