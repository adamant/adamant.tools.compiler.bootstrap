using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public sealed class PlacementInitExpressionSyntax : ExpressionSyntax
    {
        public ExpressionSyntax PlaceExpression { get; }
        public ExpressionSyntax Initializer { get; }
        public FixedList<ArgumentSyntax> Arguments { get; }

        public PlacementInitExpressionSyntax(
            TextSpan span,
            ExpressionSyntax placeExpression,
            ExpressionSyntax initializer,
            FixedList<ArgumentSyntax> arguments)
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
