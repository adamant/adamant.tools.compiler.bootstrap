using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// Convert `none` to a specific optional type
    /// </summary>
    // TODO is this needed? shouldn't `none` have the type `never?` which would have an implicit conversion?
    public interface IImplicitNoneConversionExpression : IImplicitConversionExpression
    {
        OptionalType ConvertToType { get; }
    }
}
