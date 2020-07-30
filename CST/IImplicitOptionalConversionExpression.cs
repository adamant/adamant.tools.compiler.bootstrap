using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// Convert a type to option of that type (i.e. "Some(value)")
    /// </summary>
    public interface IImplicitOptionalConversionExpression : IImplicitConversionExpression
    {
        OptionalType ConvertToType { get; }
    }
}
