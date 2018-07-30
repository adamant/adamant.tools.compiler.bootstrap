using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions.Operators
{
    public abstract class OperatorExpression : Expression
    {
        protected OperatorExpression(DataType type)
            : base(type)
        {
        }
    }
}
