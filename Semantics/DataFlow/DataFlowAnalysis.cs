using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.DataFlow
{
    /// <summary>
    /// An abstract data flow analysis. The specific analysis performed is
    /// determined by the data flow analysis strategy.
    /// </summary>
    public class DataFlowAnalysis
    {
        public static void Check<TState>(
            IDataFlowAnalysisStrategy<TState> strategy,
            FixedList<MemberDeclarationSyntax> memberDeclarations,
            Diagnostics diagnostics)
        {
            var dataFlowAnalyzer = new DataFlowAnalyzer<TState>(strategy, diagnostics);
            foreach (var memberDeclaration in memberDeclarations)
                dataFlowAnalyzer.Check(memberDeclaration);
        }
    }
}
