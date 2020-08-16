using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class NextExpression : Expression, INextExpression
    {
        public NextExpression(TextSpan span, DataType dataType)
            : base(span, dataType) { }
    }
}
