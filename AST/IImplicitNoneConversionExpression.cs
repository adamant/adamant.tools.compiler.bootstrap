using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IImplicitNoneConversionExpression : IImplicitConversionExpression
    {
        OptionalType ConvertToType { get; }
    }
}
