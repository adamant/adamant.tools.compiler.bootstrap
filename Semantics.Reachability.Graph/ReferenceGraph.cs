using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class ReferenceGraph : IReferenceGraph
    {
        private bool isDirty = false;
        private bool recomputingCurrentAccess = false;

        private readonly Dictionary<BindingSymbol, CallerVariable> callerVariables = new Dictionary<BindingSymbol, CallerVariable>();
        private readonly Dictionary<BindingSymbol, Variable> variables = new Dictionary<BindingSymbol, Variable>();
        private readonly Dictionary<IAbstractSyntax, Object> objects = new Dictionary<IAbstractSyntax, Object>();
        private readonly HashSet<TempValue> tempValues = new HashSet<TempValue>();

        public IReadOnlyCollection<CallerVariable> CallerVariables => callerVariables.Values;
        public IReadOnlyCollection<Variable> Variables => variables.Values;
        public IReadOnlyCollection<Object> Objects => objects.Values;
        public IReadOnlyCollection<TempValue> TempValues => tempValues;

        void IReferenceGraph.Dirty()
        {
            Dirty();
        }

        internal void Dirty()
        {
            isDirty = true;
        }

        public CallerVariable AddCallerVariable(BindingSymbol symbol)
        {
            var variable = new CallerVariable(this, symbol);
            callerVariables.Add(variable.Symbol, variable);
            Dirty();
            return variable;
        }

        public Variable AddVariable(BindingSymbol symbol)
        {
            var variable = new Variable(this, symbol);
            variables.Add(variable.Symbol, variable);
            Dirty();
            return variable;
        }

        public TempValue AddTempValue(IExpression expression, ReferenceType referenceType)
        {
            var tempValue = new TempValue(this, expression, referenceType);
            tempValues.Add(tempValue);
            Dirty();
            return tempValue;
        }

        public Reference AddNewObject(
            Ownership ownership,
            Access declaredAccess,
            IAbstractSyntax syntax,
            bool isContext,
            bool isOriginOfMutability)
        {
            var reference = Reference.ToNewObject(this, ownership, declaredAccess, syntax, isContext, isOriginOfMutability);
            var referent = reference.Referent;
            objects.Add(referent.OriginSyntax, referent);
            Dirty();
            return reference;
        }

        public void EnsureCurrentAccessIsUpToDate()
        {
            if (recomputingCurrentAccess) throw new Exception("Current access is being updated");
            if (!isDirty) return;

            try
            {
                recomputingCurrentAccess = true;
                RecomputeCurrentObjectAccess();
                isDirty = false;
            }
            finally
            {
                recomputingCurrentAccess = false;
            }
        }

        public void EndVariableScope(BindingSymbol variable)
        {
            if (!variables.Remove(variable, out var place))
                throw new Exception($"Variable '{variable.Name}' does not exist in the graph.");

            place.Freed();
            Dirty();
        }

        public void Drop(TempValue? temp)
        {
            if (temp is null) return;
            if (!tempValues.Remove(temp)) throw new Exception($"Temp value '{temp}' does not exist in the graph.");

            temp.Freed();
            Dirty();
        }

        public void Delete(Object obj)
        {
            if (obj.Graph != this) throw new Exception($"Object '{obj}' is from a different graph.");

            if (objects.ContainsKey(obj.OriginSyntax)) obj.Freed();
        }

        /// <summary>
        /// A reference to a given object was lost, it may not be in the graph anymore
        /// </summary>
        public void LostReference(Object obj)
        {
            if (obj.Graph != this) throw new Exception($"Object '{obj}' is from a different graph.");

            if (!(obj.GetCurrentAccess() is null)) return;

            Delete(obj);
        }

        /// <summary>
        /// Assigns a temp value into a variable, consuming the temp value.
        /// </summary>
        /// <remarks>This is a method of the graph so that the temp value
        /// can be removed from the graph even if there is no variable.</remarks>
        public void Assign(Variable? variable, TempValue? value)
        {
            if (value == null) return;
            variable?.Assign(value);
            Drop(value);
        }

        public Variable GetVariableFor(BindingSymbol variableSymbol)
        {
            // Variable needs to have already been declared
            return variables[variableSymbol];
        }

        public Variable? TryGetVariableFor(BindingSymbol variableSymbol)
        {
            return variables.TryGetValue(variableSymbol, out var variable) ? variable : null;
        }

        private void RecomputeCurrentObjectAccess()
        {
            // Reset mutability to unknown (null)
            foreach (var place in Objects) place.ResetAccess();

            var rootPlaces = callerVariables.Values.SafeCast<StackPlace>().Concat(variables.Values).Concat(tempValues)
                                            .ToFixedList();

            foreach (var place in rootPlaces) place.MarkReferencedObjects();
        }
    }
}
