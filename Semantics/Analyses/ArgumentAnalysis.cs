using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class ArgumentAnalysis : SyntaxAnalysis
    {
        [NotNull] public ArgumentSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis Value { get; }

        public ArgumentAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ArgumentSyntax syntax,
            [NotNull] ExpressionAnalysis value)
            : base(context)
        {
            Requires.NotNull(nameof(value), value);
            Syntax = syntax;
            Value = value;
        }
    }
}
