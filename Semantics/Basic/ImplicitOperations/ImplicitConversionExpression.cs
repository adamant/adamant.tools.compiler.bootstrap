using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Basic.ImplicitOperations
{
    /// <summary>
    /// The implicit conversions do not have "Syntax" in their name, because
    /// there is no corresponding syntax. They are never generated by the parser,
    /// but are inserted during type checking.
    /// </summary>
    internal abstract class ImplicitConversionExpression : IImplicitConversionExpression
    {
        public TextSpan Span { get; }
        public IExpressionSyntax Expression { get; }
        public DataType Type { get; }
        DataType? IExpressionSyntax.Type
        {
            get => Type;
            set => throw new InvalidOperationException("Can't set type of ImplicitConversionExpression");
        }

        protected ImplicitConversionExpression(TextSpan span, DataType convertToType, IExpressionSyntax expression)
        {
            Span = span;
            Expression = expression;
            Type = convertToType;
        }

        public abstract override string ToString();
    }
}
