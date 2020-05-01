using System.Collections.ObjectModel;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Scopes;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A graph of the possible references between places in a function. Also
    /// answers questions about the current mutability of objects etc.
    /// </summary>
    internal class ReachabilityGraph
    {
        private readonly PlaceIdentifierList identifiers;
        private readonly LexicalVariableScope currentScope;
        private readonly PlaceDictionary<VariablePlace> variables = new PlaceDictionary<VariablePlace>();
        private readonly PlaceDictionary<FieldPlace> fields = new PlaceDictionary<FieldPlace>();
        private readonly PlaceDictionary<ObjectPlace> objects = new PlaceDictionary<ObjectPlace>();

        public ReachabilityGraph(PlaceIdentifierList identifiers, LexicalVariableScope currentScope)
        {
            this.identifiers = identifiers;
            this.currentScope = currentScope;
        }

        public ObjectPlace CallerOwnedObjectFor(IParameterSyntax parameter)
        {
            var variableIdentifier = currentScope.CallerScope.VariableDeclared(SpecialName.CallerBound(parameter.Name));
            var callerVariable = PlaceFor(variableIdentifier);
            var callerObject = PlaceFor(identifiers.ObjectIdentifierFor(parameter));
            callerVariable.OwningIdentifies(callerObject);
            return callerObject;
        }

        public VariablePlace VariableDeclared(SimpleName name)
        {
            var variable = currentScope.VariableDeclared(name);
            return PlaceFor(variable);
        }

        public VariablePlace VariableFor(SimpleName name)
        {
            var variable = identifiers.VariableIdentifierFor(name);
            return PlaceFor(variable);
        }

        public FieldPlace FieldFor(SimpleName name)
        {
            var variable = identifiers.FieldIdentifierFor(name);
            return PlaceFor(variable);
        }

        public ObjectPlace ObjectFor(IParameterSyntax parameter)
        {
            var @object = identifiers.ObjectIdentifierFor(parameter);
            return PlaceFor(@object);
        }
        public ObjectPlace ObjectFor(IExpressionSyntax expression)
        {
            var @object = identifiers.ObjectIdentifierFor(expression);
            return PlaceFor(@object);
        }

        private VariablePlace PlaceFor(VariablePlaceIdentifier variable)
        {
            if (!variables.TryGetValue(variable, out var place))
            {
                place = new VariablePlace(variable);
                variables.Add(place);
            }

            return place;
        }
        private FieldPlace PlaceFor(FieldPlaceIdentifier field)
        {
            if (!fields.TryGetValue(field, out var place))
            {
                place = new FieldPlace(field);
                fields.Add(place);
            }

            return place;
        }
        private ObjectPlace PlaceFor(ObjectPlaceIdentifier @object)
        {
            if (!objects.TryGetValue(@object, out var place))
            {
                place = new ObjectPlace(@object);
                objects.Add(place);
            }

            return place;
        }

        private class PlaceDictionary<TPlace> : KeyedCollection<PlaceIdentifier, TPlace>
            where TPlace : Place
        {
            protected override PlaceIdentifier GetKeyForItem(TPlace place)
            {
                return place.Identifier;
            }

            // TODO override Contains(TPlace) to improve its performance
        }
    }
}
