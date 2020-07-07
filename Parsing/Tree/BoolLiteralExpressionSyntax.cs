using System.Diagnostics;
using System.Globalization;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class BoolLiteralExpressionSyntax : LiteralExpressionSyntax, IBoolLiteralExpressionSyntax
    {
        public bool Value { [DebuggerStepThrough] get; }

        public BoolLiteralExpressionSyntax(TextSpan span, bool value)
            : base(span)
        {
            Value = value;
        }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }
    }
}
