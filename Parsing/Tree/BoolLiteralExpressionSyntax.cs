using System.Diagnostics;
using System.Globalization;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;

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

        public override string ToString()
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public override string ToGroupedString()
        {
            return ToString();
        }
    }
}
