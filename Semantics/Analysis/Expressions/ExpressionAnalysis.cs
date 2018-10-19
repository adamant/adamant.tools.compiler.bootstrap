using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions
{
    public abstract class ExpressionAnalysis
    {
        [NotNull] public AnalysisContext Context { get; }
        [NotNull] public ExpressionSyntax Syntax { get; }
        [CanBeNull] public DataType Type { get; internal set; }

        protected ExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] ExpressionSyntax syntax)
        {
            Requires.NotNull(nameof(context), context);
            Requires.NotNull(nameof(syntax), syntax);
            Context = context;
            Syntax = syntax;
        }
    }
}
