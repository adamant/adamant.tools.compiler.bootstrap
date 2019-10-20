using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    [Closed(
        typeof(IImplicitNumericConversionExpression),
        typeof(IImplicitOptionalConversionExpression),
        typeof(IImplicitNoneConversionExpression),
        typeof(IImplicitImmutabilityConversionExpression))]
    public interface IImplicitConversionExpression : IExpressionSyntax
    {
        IExpressionSyntax Expression { get; }
        new DataType Type { get; }
    }
}
