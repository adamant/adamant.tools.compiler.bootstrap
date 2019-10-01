using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface IImplicitOptionalConversionExpression : IImplicitConversionExpression
    {
        OptionalType ConvertToType { get; }
    }
}
