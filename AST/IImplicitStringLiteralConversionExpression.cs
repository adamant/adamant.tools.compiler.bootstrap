using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// Implicit conversion from string literal to string object
    /// </summary>
    public interface IImplicitStringLiteralConversionExpression : IImplicitConversionExpression
    {
        new IStringLiteralExpressionSyntax Expression { get; }
        ISymbol ConversionFunction { get; }
    }
}
