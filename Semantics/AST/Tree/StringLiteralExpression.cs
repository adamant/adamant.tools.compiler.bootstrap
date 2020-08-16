using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class StringLiteralExpression : LiteralExpression, IStringLiteralExpression
    {
        public string Value { get; }

        public StringLiteralExpression(TextSpan span, DataType dataType, string value)
            : base(span, dataType)
        {
            Value = value;
        }
    }
}
