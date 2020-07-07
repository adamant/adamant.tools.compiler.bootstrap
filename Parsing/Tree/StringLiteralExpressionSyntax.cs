using System.Diagnostics;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class StringLiteralExpressionSyntax : LiteralExpressionSyntax, IStringLiteralExpressionSyntax
    {
        public string Value { [DebuggerStepThrough] get; }

        public StringLiteralExpressionSyntax(TextSpan span, string value)
            : base(span)
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
