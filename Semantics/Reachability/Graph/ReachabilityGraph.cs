using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A graph of the possible references between places in a function. Also
    /// answers questions about the current mutability of objects etc.
    /// </summary>
    internal class ReachabilityGraph
    {
        private readonly LexicalVariableScope currentScope;
        private readonly Dictionary<IBindingSymbol, VariablePlace> variables = new Dictionary<IBindingSymbol, VariablePlace>();
        private readonly Dictionary<IBindingSymbol, FieldPlace> fields = new Dictionary<IBindingSymbol, FieldPlace>();
        private readonly Dictionary<ISyntax, ObjectPlace> objects = new Dictionary<ISyntax, ObjectPlace>();

        public ReachabilityGraph(LexicalVariableScope currentScope)
        {
            this.currentScope = currentScope;
        }

        public ObjectPlace CallerObjectFor(IParameterSyntax parameter)
        {
            var callerVariable = currentScope.CallerScope.CallerVariable(parameter);
            var objectPlace = ObjectFor(parameter);
            callerVariable.Assign(objectPlace);
            return objectPlace;
        }

        public VariablePlace VariableDeclared(IBindingSymbol variableSymbol)
        {
            currentScope.VariableDeclared(variableSymbol);
            var place = new VariablePlace(variableSymbol);
            variables.Add(place.Symbol, place);
            return place;
        }
        public VariablePlace VariableFor(IBindingSymbol variableSymbol)
        {
            // Variable needs to have already been declared
            return variables[variableSymbol];
        }

        public FieldPlace FieldFor(IBindingSymbol fieldSymbol)
        {
            if (!fields.TryGetValue(fieldSymbol, out var place))
            {
                place = new FieldPlace(fieldSymbol);
                fields.Add(place.Symbol, place);
            }

            return place;
        }

        public ObjectPlace ObjectFor(IParameterSyntax parameter)
        {
            if (!objects.TryGetValue(parameter, out var place))
            {
                place = new ObjectPlace(parameter);
                objects.Add(place.OriginSyntax, place);
            }

            return place;
        }
        public ObjectPlace ObjectFor(IExpressionSyntax expression)
        {
            if (!objects.TryGetValue(expression, out var place))
            {
                place = new ObjectPlace(expression);
                objects.Add(place.OriginSyntax, place);
            }

            return place;
        }
    }
}
