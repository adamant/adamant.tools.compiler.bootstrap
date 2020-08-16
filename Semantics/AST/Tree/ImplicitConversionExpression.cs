using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal abstract class ImplicitConversionExpression : Expression, IImplicitConversionExpression
    {
        public IExpression Expression { get; }

        protected ImplicitConversionExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            IExpression expression)
            : base(span, dataType, semantics)
        {
            Expression = expression;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;
    }
}
