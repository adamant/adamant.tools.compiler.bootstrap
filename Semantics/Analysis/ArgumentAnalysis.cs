using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis
{
    public class ArgumentAnalysis : SyntaxAnalysis
    {
        [NotNull] public new ArgumentSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis Value { get; }

        public ArgumentAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ArgumentSyntax syntax,
            [NotNull] ExpressionAnalysis value)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(value), value);
            Syntax = syntax;
            Value = value;
        }
    }
}
