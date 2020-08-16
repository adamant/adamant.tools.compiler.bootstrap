using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class BreakExpression : Expression, IBreakExpression
    {
        public IExpression? Value { get; }

        public BreakExpression(TextSpan span, DataType dataType, IExpression? value)
            : base(span, dataType)
        {
            Value = value;
        }
    }
}
