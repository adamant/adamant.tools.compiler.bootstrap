using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
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
        private readonly Dictionary<IBindingSymbol, Variable> variables = new Dictionary<IBindingSymbol, Variable>();

        private readonly Dictionary<ISyntax, ContextObject> contextObjects = new Dictionary<ISyntax, ContextObject>();
        private readonly Dictionary<ISyntax, Object> objects = new Dictionary<ISyntax, Object>();

        private readonly HashSet<TempValue> tempValues = new HashSet<TempValue>();

        #region Add/Remove Methods
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
                    if (contextObjects.TryAdd(contextObject.OriginSyntax, contextObject))
                        AddReferences(contextObject);
                    break;
                case Object @object:
                    if (objects.TryAdd(@object.OriginSyntax, @object))
                        AddReferences(@object);
                    break;
            }
        }

        public void Add(TempValue? temp)
        {
            if (temp is null) return; // for convenience
            if (tempValues.Add(temp))
                AddReferences(temp);
        }

        private void AddReferences(Place place)
        {
            foreach (var reference in place.References)
                Add(reference.Referent);
        }

        public bool FreeVariable(IBindingSymbol variable)
        {
            if (!variables.Remove(variable, out var place)) return false;
            place.Free();
            return true;
        }

        public bool Remove(TempValue tempValue)
        {
            // Make sure this is released
            tempValue.Free();
            return tempValues.Remove(tempValue);
        }

        public void Remove(IEnumerable<TempValue?> values)
        {
            foreach (var tempValue in values)
                if (!(tempValue is null))
                    Remove(tempValue);
        }
        #endregion

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

        public void ComputeCurrentObjectAccess()
        {
            var heapPlaces = AllHeapPlaces().ToFixedList();

            // Reset mutability to unknown (null)
            foreach (var place in heapPlaces)
                place.ResetAccess();

            var rootPlaces = callerVariables.Values.SafeCast<StackPlace>()
                                            .Concat(variables.Values)
                                            .Concat(tempValues)
                                            .ToFixedList();

            foreach (var place in rootPlaces)
                place.MarkReferencedObjects();
        }

        private IEnumerable<HeapPlace> AllHeapPlaces()
        {
            return contextObjects.Values.SafeCast<HeapPlace>().Concat(objects.Values);
        }
    }
}
