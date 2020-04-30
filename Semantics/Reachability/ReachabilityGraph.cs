using System.Collections.ObjectModel;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    /// <summary>
    /// A graph of the possible references between places in a function. Also
    /// answers questions about the current mutability of objects etc.
    /// </summary>
    internal class ReachabilityGraph
    {
        private readonly PlaceList places;
        private readonly VariablePlaceScope variableScope;
        private readonly NodeDictionary<VariablePlace> variables = new NodeDictionary<VariablePlace>();
        private readonly NodeDictionary<ObjectPlace> objects = new NodeDictionary<ObjectPlace>();

        public ReachabilityGraph(PlaceList places, VariablePlaceScope variableScope)
        {
            this.places = places;
            this.variableScope = variableScope;
        }

        public Node<ObjectPlace> CallerOwnedObjectFor(IParameterSyntax parameter)
        {
            var callerVariable = GetOrAdd(variableScope.CallerScope.VariableDeclared(SpecialName.CallerBound(parameter.Name)));
            var callerObject = GetOrAdd(places.GetOrAddObject(parameter));
            callerVariable.Owns(callerObject);
            return callerObject;
        }

        public Node<VariablePlace> VariableDeclared(SimpleName name)
        {
            var variable = variableScope.VariableDeclared(name);
            return GetOrAdd(variable);
        }

        public Node<ObjectPlace> ObjectFor(IParameterSyntax parameter)
        {
            var @object = places.GetOrAddObject(parameter);
            return GetOrAdd(@object);
        }

        private Node<VariablePlace> GetOrAdd(VariablePlace variable)
        {
            if (!variables.TryGetValue(variable, out var node))
            {
                node = new Node<VariablePlace>(variable);
                variables.Add(node);
            }

            return node;
        }
        private Node<ObjectPlace> GetOrAdd(ObjectPlace @object)
        {
            if (!objects.TryGetValue(@object, out var node))
            {
                node = new Node<ObjectPlace>(@object);
                objects.Add(node);
            }

            return node;
        }

        public class Node<TPlace>
            where TPlace : Place
        {
            public TPlace Place { get; }

            public Node(TPlace place)
            {
                Place = place;
            }

            public void Owns<TOtherPlace>(Node<TOtherPlace> place)
                where TOtherPlace : Place
            {
                throw new System.NotImplementedException();
            }

            public void Shares<TOtherPlace>(Node<TOtherPlace> place)
                where TOtherPlace : Place
            {
                throw new System.NotImplementedException();
            }

            public void PotentiallyOwns<TOtherPlace>(Node<TOtherPlace> place)
                where TOtherPlace : Place
            {
                throw new System.NotImplementedException();
            }

            public void Borrows<TOtherPlace>(Node<TOtherPlace> place)
                where TOtherPlace : Place
            {
                throw new System.NotImplementedException();
            }

            public void Identifies<TOtherPlace>(Node<TOtherPlace> place)
                where TOtherPlace : Place
            {
                throw new System.NotImplementedException();
            }
        }

        private class NodeDictionary<TPlace> : KeyedCollection<Place, Node<TPlace>>
            where TPlace : Place
        {
            protected override Place GetKeyForItem(Node<TPlace> node)
            {
                return node.Place;
            }

            // TODO override Contains(PlaceNode) to improve its performance
        }



    }
}
