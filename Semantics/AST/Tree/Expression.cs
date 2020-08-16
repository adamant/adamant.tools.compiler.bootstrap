using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal abstract class Expression : AbstractSyntax, IExpression
    {
        public DataType DataType { get; }
        public ExpressionSemantics Semantics { get; }

        protected Expression(TextSpan span, DataType dataType, ExpressionSemantics semantics)
            : base(span)
        {
            DataType = dataType;
            Semantics = semantics;
        }

        protected abstract OperatorPrecedence ExpressionPrecedence { get; }

        public string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        {
            return surroundingPrecedence > ExpressionPrecedence ? $"({this})" : ToString();
        }
    }
}
