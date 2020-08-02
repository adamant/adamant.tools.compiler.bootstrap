using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// Implicit conversion between simple numeric types
    /// </summary>
    public partial interface IImplicitNumericConversionExpression : IImplicitConversionExpression
    {
        NumericType ConvertToType { get; }
    }
}
