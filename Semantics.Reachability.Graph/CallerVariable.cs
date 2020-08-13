using System;
using Adamant.Tools.Compiler.Bootstrap.CST;
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
        public ILocalBindingSyntax Syntax { get; }

        private CallerVariable(ReachabilityGraph graph, ILocalBindingSyntax syntax)
            : base(graph)
        {
            _ = syntax.BindingDataType.UnderlyingReferenceType()
                ?? throw new ArgumentException("Must be a reference type", nameof(syntax));
            Syntax = syntax;
        }

        internal static CallerVariable CreateForParameterWithObject(ReachabilityGraph graph, IBindingParameterSyntax parameter)
        {
            var reference = Reference.ToNewParameterContextObject(graph, parameter);
            var callerVariable = new CallerVariable(graph, parameter);
            callerVariable.AddReference(reference);
            return callerVariable;
        }

        public override string ToString()
        {
            return $"⟦{Syntax.Symbol.Result.Name}⟧: {Syntax.BindingDataType}";
        }
    }
}
