using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class BreakExpression : Expression, IBreakExpression
    {
        public IExpression? Value { get; }

        public BreakExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            IExpression? value)
            : base(span, dataType, semantics)
        {
            Value = value;
        }

        protected override OperatorPrecedence ExpressionPrecedence =>
            Value != null ? OperatorPrecedence.Min : OperatorPrecedence.Primary;

        public override string ToString()
        {
            if (Value != null) return $"break {Value}";
            return "break";
        }
    }
}
