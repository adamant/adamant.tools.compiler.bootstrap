using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ImplicitOptionalConversionExpression : ImplicitConversionExpression, IImplicitOptionalConversionExpression
    {
        public OptionalType ConvertToType { get; }

        public ImplicitOptionalConversionExpression(
            TextSpan span,
            DataType dataType,
            IExpression expression,
            OptionalType convertToType)
            : base(span, dataType, expression)
        {
            ConvertToType = convertToType;
        }
    }
}
