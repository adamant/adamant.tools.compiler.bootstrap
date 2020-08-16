using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ImplicitImmutabilityConversion : ImplicitConversionExpression, IImplicitImmutabilityConversionExpression
    {
        public ObjectType ConvertToType { get; }

        public ImplicitImmutabilityConversion(
            TextSpan span,
            DataType dataType,
            IExpression expression,
            ObjectType convertToType)
            : base(span, dataType, expression)
        {
            ConvertToType = convertToType;
        }
    }
}
