using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class BoolLiteralExpression : LiteralExpression, IBoolLiteralExpression
    {
        public bool Value { get; }

        public BoolLiteralExpression(TextSpan span, DataType dataType, bool value)
            : base(span, dataType)
        {
            Value = value;
        }
    }
}
