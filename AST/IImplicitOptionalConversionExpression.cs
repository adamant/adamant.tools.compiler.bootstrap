using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Convert a type to option of that type (i.e. "Some(value)")
    /// </summary>
    public interface IImplicitOptionalConversionExpression : IImplicitConversionExpression
    {
        OptionalType ConvertToType { get; }
    }
}
