using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class UnsafeExpression : Expression, IUnsafeExpression
    {
        public IExpression Expression { get; }

        public UnsafeExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            IExpression expression)
            : base(span, dataType, semantics)
        {
            Expression = expression;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"unsafe ({Expression})";
        }
    }
}
