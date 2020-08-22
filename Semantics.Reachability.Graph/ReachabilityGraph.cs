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
    public class ReachabilityGraph
    {
        internal IReferenceGraph ReferenceGraph { get; } = new ReferenceGraph();

        #region Add/Remove Methods
        /// <returns>The added local variable for the parameter</returns>
        public Variable? AddParameter(IBindingParameter parameter)
        {
            // Non-reference types don't participate in reachability (yet)
            var referenceType = parameter.Symbol.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null)
                return null;

            var localVariable = ReferenceGraph.AddVariable(parameter.Symbol);

            var capability = referenceType.ReferenceCapability;
            switch (capability)
            {
                default:
                    throw ExhaustiveMatch.Failed(capability);
                case ReferenceCapability.IsolatedMutable:
                case ReferenceCapability.Isolated:
                {
                    // Isolated parameters are fully independent of the caller
                    var reference = ReferenceToNewParameterObject(parameter);
                    localVariable.AddReference(reference);
                }
                break;
                case ReferenceCapability.Owned:
                case ReferenceCapability.OwnedMutable:
                case ReferenceCapability.Held:
                case ReferenceCapability.HeldMutable:
                {
                    var reference = ReferenceToNewParameterObject(parameter);
                    localVariable.AddReference(reference);
                    var referencedObject = reference.Referent;

                    // TODO this context object is needed
                    // Object to represent the bounding of the lifetime
                    //var callerVariable = AddCallerVariableForParameterWithObject(parameter);
                    //referencedObject.ShareFrom(callerVariable);
                }
                break;
                case ReferenceCapability.Borrowed:
                {
                    var callerVariable = AddCallerVariableForParameterWithObject(parameter);
                    localVariable.BorrowFrom(callerVariable);
                }
                break;
                case ReferenceCapability.Shared:
                {
                    var callerVariable = AddCallerVariableForParameterWithObject(parameter);
                    localVariable.ShareFrom(callerVariable);
                }
                break;
                case ReferenceCapability.Identity:
                {
                    var callerVariable = AddCallerVariableForParameterWithObject(parameter);
                    localVariable.IdentityFrom(callerVariable);
                }
                break;
            }

            return localVariable;
        }

        private CallerVariable AddCallerVariableForParameterWithObject(IBindingParameter parameter)
        {
            var reference = ReferenceToNewParameterContextObject(parameter);
            var callerVariable = ReferenceGraph.AddCallerVariable(parameter.Symbol);
            callerVariable.AddReference(reference);
            return callerVariable;
        }

        public Variable? AddField(IFieldDeclaration fieldDeclaration)
        {
            // Non-reference types don't participate in reachability (yet)
            var referenceType = fieldDeclaration.Symbol.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var variable = ReferenceGraph.AddVariable(fieldDeclaration.Symbol);
            variable.AddReference(ReferenceToNewFieldObject(fieldDeclaration));
            return variable;
        }

        public Variable? AddVariable(IVariableDeclarationStatement variableDeclaration)
        {
            var referenceType = variableDeclaration.Symbol.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            return ReferenceGraph.AddVariable(variableDeclaration.Symbol);
        }

        public TempValue? AddObject(INewObjectExpression exp)
        {
            var referenceType = exp.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = ReferenceToNewObject(exp);
            var temp = ReferenceGraph.AddTempValue(exp, referenceType);

            temp.AddReference(reference);
            return temp;
        }

        public TempValue? AddLiteral(IStringLiteralExpression exp)
        {
            var referenceType = exp.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = ReferenceToNewContextObject(exp);
            var temp = ReferenceGraph.AddTempValue(exp, referenceType);
            temp.AddReference(reference);

            return temp;
        }

        public TempValue? AddFunctionCall(IInvocationExpression exp)
        {
            var referenceType = exp.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = ReferenceToNewInvocationReturnedObject(exp);
            var temp = ReferenceGraph.AddTempValue(exp, referenceType);
            temp.AddReference(reference);
            return temp;
        }

        public TempValue? AddFieldAccess(IFieldAccessExpression fieldAccess)
        {
            var referenceType = fieldAccess.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = ReferenceToFieldAccess(fieldAccess);
            var temp = ReferenceGraph.AddTempValue(fieldAccess, referenceType);
            temp.AddReference(reference);
            return temp;
        }

        public TempValue? AddReturnValue(IExpression expression, DataType type)
        {
            var referenceType = type.UnderlyingReferenceType();
            if (referenceType is null) return null;

            return ReferenceGraph.AddTempValue(expression, referenceType);
        }

        public TempValue? AddTempValue(IExpression expression)
        {
            DataType type = expression.DataType.Known();
            var referenceType = type.UnderlyingReferenceType();
            if (referenceType is null) return null;

            return ReferenceGraph.AddTempValue(expression, referenceType);
        }

        //private void Add(Object obj)
        //{
        //    if (objects.TryAdd(obj.OriginSyntax, obj))
        //    {
        //        AddReferences(obj);
        //        this.Dirty();
        //    }
        //}

        //public void Add(TempValue? temp)
        //{
        //    if (temp is null) return; // for convenience
        //    if (tempValues.Add(temp))
        //    {
        //        AddReferences(temp);
        //        Dirty();
        //    }
        //}

        //private void Add(Variable? variable)
        //{
        //    if (variable is null) return; // for convenience
        //    variables.Add(variable.Symbol, variable);
        //    AddReferences(variable);
        //    Dirty();
        //}

        //private void AddReferences(MemoryPlace place)
        //{
        //    foreach (var reference in place.References)
        //        Add(reference.Referent);
        //}

        public void EndVariableScope(BindingSymbol variable)
        {
            ReferenceGraph.EndVariableScope(variable);
        }

        public void Drop(TempValue? temp)
        {
            ReferenceGraph.Drop(temp);
        }

        public void ExitFunction(TempValue? returnValue)
        {
            foreach (var tempValue in ReferenceGraph.TempValues.Except(returnValue).ToList())
                ReferenceGraph.Drop(tempValue);
            foreach (var variable in ReferenceGraph.Variables.ToList())
                ReferenceGraph.EndVariableScope(variable.Symbol);
        }

        public void Drop(IEnumerable<TempValue?> temps)
        {
            foreach (var tempValue in temps) Drop(tempValue);
        }
        #endregion

        /// <summary>
        /// Assigns a temp value into a variable, consuming the temp value.
        /// </summary>
        /// <remarks>This is a method of the graph so that the temp value
        /// can be removed from the graph even if there is no variable.</remarks>
        public void Assign(Variable? variable, TempValue? value)
        {
            ReferenceGraph.Assign(variable, value);
        }

        public Variable GetVariableFor(BindingSymbol variableSymbol)
        {
            return ReferenceGraph.GetVariableFor(variableSymbol);
        }
        public Variable? TryGetVariableFor(BindingSymbol variableSymbol)
        {
            return ReferenceGraph.TryGetVariableFor(variableSymbol);
        }

        private Reference ReferenceToNewParameterObject(IBindingParameter parameter)
        {
            var referenceType = parameter.Symbol.DataType.Known().UnderlyingReferenceType()
                                ?? throw new ArgumentException("Must be a parameter with a reference type", nameof(parameter));

            var referenceCapability = referenceType.ReferenceCapability;
            var ownership = referenceCapability.ToOwnership();
            var access = referenceCapability.ToAccess();
            var isOriginOfMutability = access == Access.Mutable;
            return ReferenceGraph.AddNewObject(ownership, access, parameter, false, isOriginOfMutability);
        }

        private Reference ReferenceToNewParameterContextObject(IBindingParameter parameter)
        {
            var referenceType = parameter.Symbol.DataType.Known().UnderlyingReferenceType()
                                ?? throw new ArgumentException("Must be a parameter with a reference type",
                                    nameof(parameter));

            var referenceCapability = referenceType.ReferenceCapability;
            var ownership = referenceCapability.ToOwnership();
            var access = referenceCapability.ToAccess();
            var isOriginOfMutability = access == Access.Mutable;
            return ReferenceGraph.AddNewObject(ownership, access, parameter, true, isOriginOfMutability);
        }

        private Reference ReferenceToNewContextObject(IExpression expression)
        {
            var referenceType = expression.DataType.Known().UnderlyingReferenceType()
                                ?? throw new ArgumentException("Must be a parameter with a reference type",
                                    nameof(expression));

            var referenceCapability = referenceType.ReferenceCapability;
            var ownership = referenceCapability.ToOwnership();
            var access = referenceCapability.ToAccess();
            var isOriginOfMutability = access == Access.Mutable;
            return ReferenceGraph.AddNewObject(ownership, access, expression, true, isOriginOfMutability);
        }

        private Reference ReferenceToNewObject(INewObjectExpression expression)
        {
            return ReferenceToExpressionObject(expression);
        }

        private Reference ReferenceToNewInvocationReturnedObject(IInvocationExpression expression)
        {
            return ReferenceToExpressionObject(expression);
        }

        private Reference ReferenceToFieldAccess(IFieldAccessExpression expression)
        {
            return ReferenceToExpressionObject(expression);
        }

        private Reference ReferenceToExpressionObject(IExpression expression)
        {
            var referenceType = expression.DataType.Known().UnderlyingReferenceType()
                                ?? throw new ArgumentException("Must be a parameter with a reference type",
                                    nameof(expression));

            var referenceCapability = referenceType.ReferenceCapability;
            var ownership = referenceCapability.ToOwnership();
            var access = referenceCapability.ToAccess();
            var isOriginOfMutability = access == Access.Mutable;
            return ReferenceGraph.AddNewObject(ownership, access, expression, false, isOriginOfMutability);
        }

        private Reference ReferenceToNewFieldObject(IFieldDeclaration field)
        {
            var referenceType = field.Symbol.DataType.Known().UnderlyingReferenceType()
                                ?? throw new ArgumentException("Must be a parameter with a reference type",
                                    nameof(field));

            var referenceCapability = referenceType.ReferenceCapability;
            var ownership = referenceCapability.ToOwnership();
            var access = referenceCapability.ToAccess();
            var isOriginOfMutability = access == Access.Mutable;
            return ReferenceGraph.AddNewObject(ownership, access, field, false, isOriginOfMutability);
        }
    }
}
