using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IUnaryExpressionSyntax),
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
        typeof(IBlockSyntax),
        typeof(IInvocationSyntax),
        typeof(IImplicitConversionExpression),
        typeof(IForeachExpressionSyntax),
        typeof(IIfExpressionSyntax),
        typeof(IAssignmentExpressionSyntax),
        typeof(IBreakExpressionSyntax),
        typeof(IBinaryExpressionSyntax),
        typeof(IInstanceExpressionSyntax),
        typeof(INameSyntax))]
    public interface IExpressionSyntax : ISyntax
    {
        DataType? Type { get; set; }
    }
}
