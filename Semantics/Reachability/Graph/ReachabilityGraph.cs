using System.Collections.ObjectModel;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Scopes;
using static Adamant.Tools.Compiler.Bootstrap.Metadata.Types.ReferenceCapability;

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

        public ObjectPlace CallerObjectFor(IParameterSyntax parameter, ReferenceType? type = null)
        {
            var identifier = currentScope.CallerScope.VariableDeclared(SpecialName.CallerBound(parameter.Name));
            type = type?.WithCapability(Identity) ?? new AnyType(Identity);
            var place = new VariablePlace(identifier, type);
            var objectPlace = PlaceFor(identifiers.ObjectIdentifierFor(parameter));
            place.Assign(objectPlace);
            return objectPlace;
        }

        public ObjectPlace CallerObjectForSelf(IConstructorDeclarationSyntax constructor, ReferenceType selfType)
        {
            var identifier = currentScope.CallerScope.VariableDeclared(SpecialName.CallerBound(SpecialName.Self));
            selfType = selfType.WithCapability(Identity);
            var place = new VariablePlace(identifier, selfType);
            var objectPlace = PlaceFor(identifiers.ObjectIdentifierFor(constructor));
            place.Assign(objectPlace);
            return objectPlace;
        }

        public VariablePlace VariableDeclared(SimpleName name, ReferenceType type)
        {
            var identifier = currentScope.VariableDeclared(name);
            var place = new VariablePlace(identifier, type);
            variables.Add(place);
            return place;
        }
        public VariablePlace VariableFor(SimpleName name)
        {
            var identifier = identifiers.VariableIdentifierFor(name);
            // Variable needs to have already been declared
            return variables[identifier];
        }

        public FieldPlace FieldDeclared(SimpleName name, ReferenceType type)
        {
            var identifier = identifiers.FieldIdentifierFor(name);
            var place = new FieldPlace(identifier, type);
            fields.Add(place);
            return place;
        }
        public FieldPlace FieldFor(SimpleName name)
        {
            var identifier = identifiers.FieldIdentifierFor(name);
            // Field needs to have already been declared
            return fields[identifier];
        }

        public ObjectPlace ObjectFor(IParameterSyntax parameter)
        {
            var identifier = identifiers.ObjectIdentifierFor(parameter);
            return PlaceFor(identifier);
        }
        public ObjectPlace ObjectFor(IExpressionSyntax expression)
        {
            var identifier = identifiers.ObjectIdentifierFor(expression);
            return PlaceFor(identifier);
        }

        //private VariablePlace PlaceFor(VariablePlaceIdentifier variable)
        //{
        //    if (!variables.TryGetValue(variable, out var place))
        //    {
        //        place = new VariablePlace(variable);
        //        variables.Add(place);
        //    }

        //    return place;
        //}
        //private FieldPlace PlaceFor(FieldPlaceIdentifier field)
        //{
        //    if (!fields.TryGetValue(field, out var place))
        //    {
        //        place = new FieldPlace(field);
        //        fields.Add(place);
        //    }

        //    return place;
        //}
        private ObjectPlace PlaceFor(ObjectPlaceIdentifier objectIdentifier)
        {
            if (!objects.TryGetValue(objectIdentifier, out var place))
            {
                place = new ObjectPlace(objectIdentifier);
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
