using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ImplicitNoneConversionExpression : ImplicitConversionExpression, IImplicitNoneConversionExpression
    {
        public OptionalType ConvertToType { get; }

        public ImplicitNoneConversionExpression(
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
