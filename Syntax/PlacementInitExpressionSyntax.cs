using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Syntax
{
    public class PlacementInitExpressionSyntax : ExpressionSyntax
    {
        [NotNull] public ExpressionSyntax PlaceExpression { get; }
        [NotNull] public NameSyntax Initializer { get; }
        [NotNull] public FixedList<ArgumentSyntax> Arguments { get; }

        public PlacementInitExpressionSyntax(
            TextSpan span,
            [NotNull] ExpressionSyntax placeExpression,
            [NotNull] NameSyntax initializer,
            [NotNull] FixedList<ArgumentSyntax> arguments)
            : base(span)
        {
            PlaceExpression = placeExpression;
            Initializer = initializer;
            Arguments = arguments;
        }

        public override string ToString()
        {
            return $"init({PlaceExpression}) {Initializer}({Arguments})";
        }
    }
}
