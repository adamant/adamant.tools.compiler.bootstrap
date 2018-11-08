using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Syntax.Nodes;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Analyses
{
    public class PlacementInitExpressionAnalysis : ExpressionAnalysis
    {
        [NotNull] public new PlacementInitExpressionSyntax Syntax { get; }
        [NotNull] public ExpressionAnalysis PlaceExpression { get; set; }
        [NotNull] public ExpressionAnalysis InitializerExpression { get; set; }
        [NotNull, ItemNotNull] public IReadOnlyList<ArgumentAnalysis> Arguments { get; }

        public PlacementInitExpressionAnalysis(
            [NotNull] AnalysisContext context,
            [NotNull] PlacementInitExpressionSyntax syntax,
            [NotNull] ExpressionAnalysis placeExpression,
            [NotNull] ExpressionAnalysis initializerExpression,
            [NotNull, ItemNotNull] IEnumerable<ArgumentAnalysis> arguments)
            : base(context, syntax)
        {
            Requires.NotNull(nameof(placeExpression), placeExpression);
            Requires.NotNull(nameof(initializerExpression), initializerExpression);
            Requires.NotNull(nameof(arguments), arguments);
            Syntax = syntax;
            PlaceExpression = placeExpression;
            InitializerExpression = initializerExpression;
            Arguments = arguments.ToReadOnlyList();
        }
    }
}
