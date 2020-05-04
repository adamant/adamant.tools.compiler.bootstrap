using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A graph of the possible references between places in a function. Also
    /// answers questions about the current mutability of objects etc.
    /// </summary>
    internal class ReachabilityGraph
    {
        private readonly Dictionary<IBindingSymbol, CallerVariable> callerVariables = new Dictionary<IBindingSymbol, CallerVariable>();
        private readonly Dictionary<IParameterSyntax, ContextObject> contextObjects = new Dictionary<IParameterSyntax, ContextObject>();
        private readonly Dictionary<IBindingSymbol, Variable> variables = new Dictionary<IBindingSymbol, Variable>();
        private readonly Dictionary<ISyntax, Object> objects = new Dictionary<ISyntax, Object>();
        private readonly HashSet<TempValue> tempValues = new HashSet<TempValue>();

        #region Add Methods
        public void Add(CallerVariable? callerVariable)
        {
            if (callerVariable is null) return; // for convenience
            callerVariables.Add(callerVariable.Symbol, callerVariable);
            AddReferences(callerVariable);
        }
        public void Add(Variable? variable)
        {
            if (variable is null) return; // for convenience
            variables.Add(variable.Symbol, variable);
            AddReferences(variable);
        }
        public void Add(HeapPlace place)
        {
            switch (place)
            {
                default:
                    throw ExhaustiveMatch.Failed(place);
                case ContextObject contextObject:
                    if (contextObjects.TryAdd(contextObject.ForParameter, contextObject))
                        AddReferences(contextObject);
                    break;
                case Object @object:
                    if (objects.TryAdd(@object.OriginSyntax, @object))
                        AddReferences(@object);
                    break;
            }
        }

        private void AddReferences(Place place)
        {
            foreach (var reference in place.References)
                Add(reference.Referent);
        }
        #endregion

        //private CallerVariable CallerVariableFor(IParameterSyntax parameter)
        //{
        //    if (!callerVariables.TryGetValue(parameter, out var place))
        //    {
        //        place = new CallerVariable(parameter);
        //        callerVariables.Add(place.Symbol, place);
        //    }

        //    return place;
        //}
        //private ContextObject ContextObjectFor(IParameterSyntax parameter)
        //{
        //    if (!contextObjects.TryGetValue(parameter, out var place))
        //    {
        //        place = new ContextObject(parameter);
        //        contextObjects.Add(place.ForParameter, place);
        //    }

        //    return place;
        //}

        //public CallerVariable CallerVariableAndObjectFor(IParameterSyntax parameter)
        //{
        //    var callerVariable = CallerVariableFor(parameter);
        //    var contextObject = ContextObjectFor(parameter);
        //    callerVariable.Owns(contextObject, true);
        //    return callerVariable;
        //}

        //public Variable VariableDeclared(IBindingSymbol variableSymbol)
        //{
        //    currentScope.VariableDeclared(variableSymbol);
        //    var place = new Variable(variableSymbol);
        //    variables.Add(place.Symbol, place);
        //    return place;
        //}

        public Variable VariableFor(IBindingSymbol variableSymbol)
        {
            // Variable needs to have already been declared
            return variables[variableSymbol];
        }
        public Variable? TryVariableFor(IBindingSymbol variableSymbol)
        {
            return variables.TryGetValue(variableSymbol, out var variable)
                ? variable : null;
        }

        //public Object ObjectFor(IParameterSyntax parameter)
        //{
        //    if (!objects.TryGetValue(parameter, out var place))
        //    {
        //        place = new Object(parameter);
        //        objects.Add(place.OriginSyntax, place);
        //    }

        //    return place;
        //}
        //public Object ObjectFor(IExpressionSyntax expression)
        //{
        //    if (!objects.TryGetValue(expression, out var place))
        //    {
        //        place = new Object(expression);
        //        objects.Add(place.OriginSyntax, place);
        //    }

        //    return place;
        //}

        public TempValue NewTempValue(ReferenceType referenceType)
        {
            var temp = new TempValue(referenceType);
            tempValues.Add(temp);
            return temp;
        }

        public void ComputeObjectStates()
        {
            var heapPlaces = AllHeapPlaces().ToFixedList();

            // Clear states
            foreach (var place in heapPlaces)
                place.State = null;

            var rootPlaces = callerVariables.Values.ImplicitCast<RootPlace>()
                                            .Concat(variables.Values)
                                            .Concat(tempValues)
                                            .ToFixedList();
            foreach (var place in rootPlaces)
                ComputeSharedObjects(place);

            foreach (var place in rootPlaces)
                MarkReferencedMutablePlaces(place);

            // Places that can't be reached have been released
            foreach (var heapPlace in heapPlaces.Where(p => p.State is null))
                heapPlace.State = ObjectState.Released;
        }

        private IEnumerable<HeapPlace> AllHeapPlaces()
        {
            return contextObjects.Values.ImplicitCast<HeapPlace>().Concat(objects.Values);
        }

        private static void ComputeSharedObjects(RootPlace place)
        {
            var readOnlyReferences = place.References
                                  .Where(r => r.IsUsed && r.DeclaredAccess == Access.ReadOnly);
            foreach (var reference in readOnlyReferences)
                MarkReadOnly(reference.Referent);
        }

        private static void MarkReadOnly(HeapPlace place)
        {
            if (place.State == ObjectState.ReadOnly)
                return; // Already readonly, don't recur through it again

            place.State = ObjectState.ReadOnly;
            // Recur to all used references which can read from their objects
            var references = place.References.Where(r => r.IsUsed  && r.DeclaredReadable);
            foreach (var reference in references)
                MarkReadOnly(reference.Referent);
        }

        private static void MarkReferencedMutablePlaces(Place place)
        {
            foreach (var reference in place.References)
            {
                var effectiveAccess = reference.EffectiveAccess();
                var referent = reference.Referent;
                switch (effectiveAccess)
                {
                    default:
                        throw ExhaustiveMatch.Failed(effectiveAccess);
                    case Access.ReadOnly:
                        // Should have already been marked
                        Debug.Assert(referent.State == ObjectState.ReadOnly);
                        break;
                    case Access.Identify:
                        if (referent.State is null)
                            referent.State = ObjectState.Mutable;
                        break;
                    case Access.Mutable:
                        referent.State = ObjectState.Mutable;
                        MarkReferencedMutablePlaces(referent);
                        break;
                }
            }
        }
    }
}
