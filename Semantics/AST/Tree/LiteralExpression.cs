using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Tokens;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal abstract class LiteralExpression : Expression, ILiteralExpression
    {
        protected LiteralExpression(TextSpan span, DataType dataType, ExpressionSemantics semantics)
            : base(span, dataType, semantics) { }

        protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;
    }
}
