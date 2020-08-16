using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class NoneLiteralExpression : LiteralExpression, INoneLiteralExpression
    {
        public NoneLiteralExpression(TextSpan span, DataType dataType, ExpressionSemantics semantics)
            : base(span, dataType, semantics) { }

        public override string ToString()
        {
            return "none";
        }
    }
}
