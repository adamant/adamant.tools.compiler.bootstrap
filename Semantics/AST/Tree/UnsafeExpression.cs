using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Core;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.AST.Tree
{
    internal class UnsafeExpression : Expression, IUnsafeExpression
    {
        public IExpression Expression { get; }

        public UnsafeExpression(TextSpan span, DataType dataType, IExpression expression)
            : base(span, dataType)
        {
            Expression = expression;
        }
    }
}
