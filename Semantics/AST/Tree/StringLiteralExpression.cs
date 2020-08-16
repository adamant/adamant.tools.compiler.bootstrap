using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class StringLiteralExpression : LiteralExpression, IStringLiteralExpression
    {
        public string Value { get; }

        public StringLiteralExpression(
            TextSpan span,
            DataType dataType,
            ExpressionSemantics semantics,
            string value)
            : base(span, dataType, semantics)
        {
            Value = value;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return $"\"{Value.Escape()}\"";
        }
    }
}
