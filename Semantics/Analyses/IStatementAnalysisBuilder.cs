using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Names;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public interface IStatementAnalysisBuilder
    {
        [NotNull]
        StatementAnalysis Build(
            [NotNull] AnalysisContext context,
            [NotNull] Name functionName,
            [NotNull] StatementSyntax statement);
    }
}
