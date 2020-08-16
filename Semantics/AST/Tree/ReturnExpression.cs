using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ReturnExpression : Expression, IReturnExpression
    {
        public IExpression? Value { get; }

        public ReturnExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            IExpression? value)
            : base(span, dataType, semantics)
        {
            Value = value;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

        public override string ToString()
        {
            return Value is null ? "return" : $"return {Value}";
        }
    }
}
