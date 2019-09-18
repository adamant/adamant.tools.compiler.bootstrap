using Adamant.Tools.Compiler.Bootstrap.Core;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    // An expression is also a statement
    [Closed(
        typeof(UnaryExpressionSyntax),
        typeof(UnsafeExpressionSyntax),
        typeof(NewObjectExpressionSyntax),
        typeof(LiteralExpressionSyntax),
        typeof(NextExpressionSyntax),
        //typeof(PlacementInitExpressionSyntax),
        typeof(ReturnExpressionSyntax),
        //typeof(TypeKindExpressionSyntax),
        typeof(WhileExpressionSyntax),
        typeof(MemberAccessExpressionSyntax),
        //typeof(MatchExpressionSyntax),
        typeof(MoveExpressionSyntax),
        typeof(LoopExpressionSyntax),
        typeof(LifetimeExpressionSyntax),
        typeof(ExpressionBlockSyntax),
        typeof(InvocationSyntax),
        typeof(ImplicitConversionExpression),
        typeof(ForeachExpressionSyntax),
        typeof(IfExpressionSyntax),
        typeof(AssignmentExpressionSyntax),
        typeof(BreakExpressionSyntax),
        typeof(BinaryExpressionSyntax),
        //typeof(DeleteExpressionSyntax),
        typeof(InstanceExpressionSyntax),
        typeof(TypeSyntax))]
    public abstract class ExpressionSyntax : StatementSyntax
    {
        public TextSpan Span { get; }

        /// <summary>
        /// If an expression has been poisoned, then it is errored in some way
        /// and we won't report errors against it in the future. We may also
        /// skip it for some processing.
        /// </summary>
        public bool Poisoned { get; private set; }

        protected ExpressionSyntax(TextSpan span)
        {
            Span = span;
        }

        public void Poison()
        {
            Poisoned = true;
        }

        // Useful for debugging
        public abstract override string ToString();
    }
}
