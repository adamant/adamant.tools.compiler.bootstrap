using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class ReturnExpression : Expression, IReturnExpression
    {
        public IExpression? Value { get; }

        public ReturnExpression(TextSpan span, DataType dataType, IExpression? value)
            : base(span, dataType)
        {
            Value = value;
        }
    }
}
