using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Implicit conversion between simple numeric types
    /// </summary>
    public interface IImplicitNumericConversionExpression : IImplicitConversionExpression
    {
        NumericType ConvertToType { get; }
    }
}