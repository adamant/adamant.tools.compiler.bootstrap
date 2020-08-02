using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(IUnaryOperatorExpressionSyntax),
        typeof(IUnsafeExpressionSyntax),
        typeof(INewObjectExpressionSyntax),
        typeof(ILiteralExpressionSyntax),
        typeof(INextExpressionSyntax),
        typeof(IReturnExpressionSyntax),
        typeof(IWhileExpressionSyntax),
        typeof(ILoopExpressionSyntax),
        typeof(IBlockExpressionSyntax),
        typeof(IInvocationExpressionSyntax),
        typeof(IImplicitConversionExpression),
        typeof(IForeachExpressionSyntax),
        typeof(IIfExpressionSyntax),
        typeof(IAssignmentExpressionSyntax),
        typeof(IBreakExpressionSyntax),
        typeof(IBinaryOperatorExpressionSyntax),
        typeof(ISelfExpressionSyntax),
        typeof(IMoveExpressionSyntax),
        typeof(IBorrowExpressionSyntax),
        typeof(IShareExpressionSyntax),
        typeof(IAssignableExpressionSyntax))]
    public partial interface IExpressionSyntax : ISyntax
    {
        [DisallowNull] DataType? Type { get; set; }
        [DisallowNull] ExpressionSemantics? Semantics { get; set; }
        string ToGroupedString(OperatorPrecedence surroundingPrecedence);
    }
}
