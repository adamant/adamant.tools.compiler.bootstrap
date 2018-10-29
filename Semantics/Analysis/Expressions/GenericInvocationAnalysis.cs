using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes.Expressions;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analysis.Expressions
{
    public class GenericInvocationAnalysis : ExpressionAnalysis
    {
        [NotNull] public ExpressionAnalysis Callee { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<ArgumentAnalysis> Arguments { get; }

        public GenericInvocationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] GenericsInvocationSyntax syntax,
            [NotNull] ExpressionAnalysis callee,
            [NotNull] [ItemNotNull] IEnumerable<ArgumentAnalysis> arguments)
            : base(context, syntax)
        {
            Callee = callee;
            Arguments = arguments.ToReadOnlyList();
        }
    }
}
