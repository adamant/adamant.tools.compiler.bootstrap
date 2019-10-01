using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Parsing.Tree
{
    internal class StringLiteralExpressionSyntax : LiteralExpressionSyntax, IStringLiteralExpressionSyntax
    {
        public string Value { get; }

        public StringLiteralExpressionSyntax(TextSpan span, string value)
            : base(span)
        {
            Value = value;
        }

        public override string ToString()
        {
            return $"\"{Value.Escape()}\"";
        }
    }
}
