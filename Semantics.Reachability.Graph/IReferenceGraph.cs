using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal interface IReferenceGraph
    {
        IReadOnlyCollection<CallerVariable> CallerVariables { get; }
        IReadOnlyCollection<Variable> Variables { get; }
        IReadOnlyCollection<Object> Objects { get; }
        IReadOnlyCollection<TempValue> TempValues { get; }
        CallerVariable AddCallerVariable(BindingSymbol symbol);
        Variable AddVariable(BindingSymbol symbol);
        TempValue AddTempValue(IExpression expression, ReferenceType referenceType);

        Reference AddNewObject(Ownership ownership, Access declaredAccess, IAbstractSyntax syntax, bool isContext, bool isOriginOfMutability);

        void EnsureCurrentAccessIsUpToDate();
        void EndVariableScope(BindingSymbol variable);
        void Drop(TempValue? temp);
        void Dirty();
        void Delete(Object obj);
        void LostReference(Object obj);
        void Assign(Variable? variable, TempValue? value);
        Variable GetVariableFor(BindingSymbol variableSymbol);
        Variable? TryGetVariableFor(BindingSymbol variableSymbol);
    }
}
