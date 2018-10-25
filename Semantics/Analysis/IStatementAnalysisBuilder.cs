using Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Statements;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Names;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Statements;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public interface IStatementAnalysisBuilder
    {
        [NotNull]
        StatementAnalysis Build(
            [NotNull] AnalysisContext context,
            [NotNull] QualifiedName functionName,
            [NotNull] StatementSyntax statement);
    }
}
