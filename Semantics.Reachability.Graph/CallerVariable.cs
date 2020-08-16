using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A caller variable is a variable that exists in the caller's stackframe.
    /// As such, it can't be mutated and must be assumed to exist for the lifetime
    /// of the function.
    /// </summary>
    public class CallerVariable : StackPlace
    {
        public BindingSymbol Symbol { get; }

        private CallerVariable(ReachabilityGraph graph, BindingSymbol symbol)
            : base(graph)
        {
            _ = symbol.DataType.UnderlyingReferenceType()
                ?? throw new ArgumentException("Must be a reference type", nameof(symbol));
            Symbol = symbol;
        }

        internal static CallerVariable CreateForParameterWithObject(ReachabilityGraph graph, IBindingParameter parameter)
        {
            var reference = Reference.ToNewParameterContextObject(graph, parameter);
            var callerVariable = new CallerVariable(graph, parameter.Symbol);
            callerVariable.AddReference(reference);
            return callerVariable;
        }

        public override string ToString()
        {
            return $"⟦{Symbol.Name}⟧: {Symbol.DataType}";
        }
    }
}
