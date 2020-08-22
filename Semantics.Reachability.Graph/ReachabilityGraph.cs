using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A graph of the possible references between places in a function. Also
    /// answers questions about the current mutability of objects etc.
    /// </summary>
    public class ReachabilityGraph : IReachabilityGraph
    {
        private bool isDirty = false;
        private bool recomputingCurrentAccess = false;
        private readonly Dictionary<BindingSymbol, CallerVariable> callerVariables = new Dictionary<BindingSymbol, CallerVariable>();
        private readonly Dictionary<BindingSymbol, Variable> variables = new Dictionary<BindingSymbol, Variable>();
        private readonly Dictionary<IAbstractSyntax, Object> objects = new Dictionary<IAbstractSyntax, Object>();
        private readonly HashSet<TempValue> tempValues = new HashSet<TempValue>();

        internal IReadOnlyCollection<CallerVariable> CallerVariables => callerVariables.Values;
        internal IReadOnlyCollection<Variable> Variables => variables.Values;
        internal IReadOnlyCollection<Object> Objects => objects.Values;
        internal IReadOnlyCollection<TempValue> TempValues => tempValues;

        void IReachabilityGraph.Dirty()
        {
            Dirty();
        }

        internal void Dirty()
        {
            isDirty = true;
        }

        void IReachabilityGraph.EnsureCurrentAccessIsUpToDate()
        {
            if (recomputingCurrentAccess)
                throw new Exception("Current access is being updated");
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

        #region Add/Remove Methods
        /// <returns>The added local variable for the parameter</returns>
        public Variable? AddParameter(IBindingParameter parameter)
        {
            // Non-reference types don't participate in reachability (yet)
            var referenceType = parameter.Symbol.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null)
                return null;

            CallerVariable? callerVariable = null;
            var localVariable = new Variable(this, parameter.Symbol);

            var capability = referenceType.ReferenceCapability;
            switch (capability)
            {
                default:
                    throw ExhaustiveMatch.Failed(capability);
                case ReferenceCapability.IsolatedMutable:
                case ReferenceCapability.Isolated:
                {
                    // Isolated parameters are fully independent of the caller
                    var reference = Reference.ToNewParameterObject(this, parameter);
                    localVariable.AddReference(reference);
                }
                break;
                case ReferenceCapability.Owned:
                case ReferenceCapability.OwnedMutable:
                case ReferenceCapability.Held:
                case ReferenceCapability.HeldMutable:
                {
                    var reference = Reference.ToNewParameterObject(this, parameter);
                    localVariable.AddReference(reference);
                    var referencedObject = reference.Referent;

                    // Object to represent the bounding of the lifetime
                    callerVariable = CallerVariable.CreateForParameterWithObject(this, parameter);
                    referencedObject.ShareFrom(callerVariable);
                }
                break;
                case ReferenceCapability.Borrowed:
                {
                    callerVariable = CallerVariable.CreateForParameterWithObject(this, parameter);
                    localVariable.BorrowFrom(callerVariable);
                }
                break;
                case ReferenceCapability.Shared:
                {
                    callerVariable = CallerVariable.CreateForParameterWithObject(this, parameter);
                    localVariable.ShareFrom(callerVariable);
                }
                break;
                case ReferenceCapability.Identity:
                {
                    callerVariable = CallerVariable.CreateForParameterWithObject(this, parameter);
                    localVariable.IdentityFrom(callerVariable);
                }
                break;
            }


            if (!(callerVariable is null))
            {
                callerVariables.Add(callerVariable.Symbol, callerVariable);
                AddReferences(callerVariable);
            }

            Add(localVariable);

            return localVariable;
        }

        public Variable? AddField(IFieldDeclaration fieldDeclaration)
        {
            var referenceType = fieldDeclaration.Symbol.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var variable = Variable.ForField(this, fieldDeclaration);
            Add(variable);
            return variable;
        }

        public Variable? AddVariable(IVariableDeclarationStatement variableDeclaration)
        {
            var referenceType = variableDeclaration.Symbol.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var variable = Variable.Declared(this, variableDeclaration);
            Add(variable);
            return variable;
        }

        public TempValue? AddObject(INewObjectExpression exp)
        {
            var temp = TempValue.ForNewObject(this, exp);
            Add(temp);
            return temp;
        }

        public TempValue? AddLiteral(IStringLiteralExpression exp)
        {
            var temp = TempValue.ForNewContextObject(this, exp);
            Add(temp);
            return temp;
        }

        public TempValue? AddFunctionCall(IInvocationExpression exp)
        {
            var temp = TempValue.ForNewInvocationReturnedObject(this, exp);
            Add(temp);
            return temp;
        }

        public TempValue? AddFieldAccess(IFieldAccessExpression fieldAccess)
        {
            var temp = TempValue.ForFieldAccess(this, fieldAccess);
            Add(temp);
            return temp;
        }

        public TempValue? AddReturnValue(IExpression expression, DataType type)
        {
            var temp = TempValue.For(this, expression, type);
            Add(temp);
            return temp;
        }

        public TempValue? AddTempValue(IExpression expression)
        {
            var temp = TempValue.For(this, expression);
            Add(temp);
            return temp;
        }

        private void Add(Object obj)
        {
            if (objects.TryAdd(obj.OriginSyntax, obj))
            {
                AddReferences(obj);
                this.Dirty();
            }
        }

        public void Add(TempValue? temp)
        {
            if (temp is null) return; // for convenience
            if (tempValues.Add(temp))
            {
                AddReferences(temp);
                Dirty();
            }
        }

        private void Add(Variable? variable)
        {
            if (variable is null) return; // for convenience
            variables.Add(variable.Symbol, variable);
            AddReferences(variable);
            Dirty();
        }

        private void AddReferences(MemoryPlace place)
        {
            foreach (var reference in place.References)
                Add(reference.Referent);
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
            if (!tempValues.Remove(temp))
                throw new Exception($"Temp value '{temp}' does not exist in the graph.");

            temp.Freed();
            Dirty();
        }

        public void ExitFunction(TempValue? returnValue)
        {
            foreach (var tempValue in TempValues.Except(returnValue).ToList()) Drop(tempValue);
            foreach (var variable in Variables.ToList()) EndVariableScope(variable.Symbol);
        }

        public void Drop(IEnumerable<TempValue?> temps)
        {
            foreach (var tempValue in temps) Drop(tempValue);
        }

        void IReachabilityGraph.Delete(Object obj)
        {
            Delete(obj);
        }

        internal void Delete(Object obj)
        {
            if (obj.Graph != this) throw new Exception($"Object '{obj}' is from a different graph.");

            if (objects.ContainsKey(obj.OriginSyntax))
                obj.Freed();
        }

        void IReachabilityGraph.LostReference(Object obj)
        {
            LostReference(obj);
        }

        /// <summary>
        /// A reference to a given object was lost, it may not be in the graph anymore
        /// </summary>
        internal void LostReference(Object obj)
        {
            if (obj.Graph != this) throw new Exception($"Object '{obj}' is from a different graph.");

            if (!(obj.GetCurrentAccess() is null)) return;

            Delete(obj);
        }
        #endregion

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
            return variables.TryGetValue(variableSymbol, out var variable)
                ? variable : null;
        }

        private void RecomputeCurrentObjectAccess()
        {
            // Reset mutability to unknown (null)
            foreach (var place in Objects)
                place.ResetAccess();

            var rootPlaces = callerVariables.Values.SafeCast<StackPlace>()
                                            .Concat(variables.Values)
                                            .Concat(tempValues)
                                            .ToFixedList();

            foreach (var place in rootPlaces)
                place.MarkReferencedObjects();
        }
    }
}
