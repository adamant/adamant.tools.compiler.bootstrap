using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;

namespace Adamant.Tools.Compiler.Bootstrap.Tests.Unit.Semantics.Fakes
{
    public class FakeStatementAnalysisBuilder : IStatementAnalysisBuilder
    {
        public StatementAnalysis Build(
            AnalysisContext context,
            QualifiedName functionName,
            StatementSyntax statement)
        {
            throw new System.NotImplementedException();
        }
    }
}
