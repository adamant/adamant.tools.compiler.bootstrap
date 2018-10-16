using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.ControlFlow
{
    public class ControlFlowGraphBuilder
    {
        public void Build([NotNull][ItemNotNull] IEnumerable<FunctionAnalysis> functionAnalyses)
        {
            functionAnalyses.ToList();
        }
    }
}
