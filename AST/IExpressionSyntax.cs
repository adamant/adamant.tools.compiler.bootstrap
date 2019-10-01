using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IUnaryOperatorExpressionSyntax),
        typeof(IUnsafeExpressionSyntax),
        typeof(INewObjectExpressionSyntax),
        typeof(ILiteralExpressionSyntax),
        typeof(INextExpressionSyntax),
        typeof(IReturnExpressionSyntax),
        typeof(IWhileExpressionSyntax),
        typeof(IMemberAccessExpressionSyntax),
        typeof(IMoveExpressionSyntax),
        typeof(ILoopExpressionSyntax),
        typeof(ILifetimeExpressionSyntax),
        typeof(IBlockExpressionSyntax),
        typeof(IInvocationExpressionSyntax),
        typeof(IImplicitConversionExpression),
        typeof(IForeachExpressionSyntax),
        typeof(IIfExpressionSyntax),
        typeof(IAssignmentExpressionSyntax),
        typeof(IBreakExpressionSyntax),
        typeof(IBinaryOperatorExpressionSyntax),
        typeof(ISelfExpressionSyntax),
        typeof(INameExpressionSyntax))]
    public interface IExpressionSyntax : ISyntax
    {
        [DisallowNull] DataType? Type { get; set; }
    }
}
