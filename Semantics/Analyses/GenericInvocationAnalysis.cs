using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class GenericInvocationAnalysis : ExpressionAnalysis
    {
        [NotNull] public new GenericsInvocationSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis Callee { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<ArgumentAnalysis> Arguments { get; }
        public int GenericArity => Arguments.Count;

        public GenericInvocationAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] GenericsInvocationSyntax syntax,
            [NotNull] ExpressionAnalysis callee,
            [NotNull] [ItemNotNull] IEnumerable<ArgumentAnalysis> arguments)
            : base(context, syntax)
        {
            Syntax = syntax;
            Callee = callee;
            Arguments = arguments.ToReadOnlyList();
        }
    }
}
