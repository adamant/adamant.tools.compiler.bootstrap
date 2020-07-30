using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    /// <summary>
    /// An abstract data flow analysis. The specific analysis performed is
    /// determined by the data flow analysis strategy.
    ///
    /// Notes:
    ///
    /// The Roslyn compiler calculates data flow by visiting through the tree in
    /// control flow order. It stores state only at labels and before loops. If
    /// it does a join on a backward edge that changes state, it makes a note of
    /// this and then makes a full pass through the function again, visiting in
    /// control flow order again.
    ///
    /// The Rust MIR proposal actually suggests checking definite assignment on
    /// the MIR. That seems like it might make it difficult to match a spec though.
    ///
    /// Given that Adamant will never have goto or arbitrary switch statements,
    /// it should be possible to do data flow analysis in a very top down way.
    /// I.e. proceed through the control flow and repeat things like loops until
    /// they stabilize. The only question might be with nested loops whether it
    /// makes sense to stabilize an inner loop before repeating an outer loop
    /// or not.
    /// </summary>
    public static class DataFlowAnalysis
    {
        public static void Check<TState>(
            IForwardDataFlowAnalyzer<TState> strategy,
            FixedList<IConcreteCallableDeclarationSyntax> callableDeclarations,
            Diagnostics diagnostics)
            where TState : class
        {
            var dataFlowAnalyzer = new ForwardDataFlowAnalyzer<TState>(strategy, diagnostics);
            foreach (var callableDeclaration in callableDeclarations)
                dataFlowAnalyzer.Walk(callableDeclaration, false);
        }

        public static void Check<TState>(
            IBackwardDataFlowAnalyzer<TState> strategy,
            FixedList<IConcreteCallableDeclarationSyntax> callableDeclarations,
            Diagnostics diagnostics)
            where TState : class
        {
            var dataFlowAnalyzer = new BackwardDataFlowAnalyzer<TState>(strategy, diagnostics);
            foreach (var callableDeclaration in callableDeclarations)
                dataFlowAnalyzer.Walk(callableDeclaration, false);
        }
    }
}
