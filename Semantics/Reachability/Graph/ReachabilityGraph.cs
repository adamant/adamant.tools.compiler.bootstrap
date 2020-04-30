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
        private readonly PlaceDictionary<ObjectPlace> objects = new PlaceDictionary<ObjectPlace>();

        public ReachabilityGraph(PlaceIdentifierList identifiers, LexicalVariableScope currentScope)
        {
            this.identifiers = identifiers;
            this.currentScope = currentScope;
        }

        public Place CallerOwnedObjectFor(IParameterSyntax parameter)
        {
            var variableIdentifier = currentScope.CallerScope.VariableDeclared(SpecialName.CallerBound(parameter.Name));
            var callerVariable = PlaceFor(variableIdentifier);
            var callerObject = PlaceFor(identifiers.ObjectIdentifierFor(parameter));
            callerVariable.Owns(callerObject);
            return callerObject;
        }

        public VariablePlace VariableDeclared(SimpleName name)
        {
            var variable = currentScope.VariableDeclared(name);
            return PlaceFor(variable);
        }

        public ObjectPlace ObjectFor(IParameterSyntax parameter)
        {
            var @object = identifiers.ObjectIdentifierFor(parameter);
            return PlaceFor(@object);
        }

        private VariablePlace PlaceFor(VariablePlaceIdentifier variable)
        {
            if (!variables.TryGetValue(variable, out var node))
            {
                node = new VariablePlace(variable);
                variables.Add(node);
            }

            return node;
        }
        private ObjectPlace PlaceFor(ObjectPlaceIdentifier @object)
        {
            if (!objects.TryGetValue(@object, out var node))
            {
                node = new ObjectPlace(@object);
                objects.Add(node);
            }

            return node;
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
