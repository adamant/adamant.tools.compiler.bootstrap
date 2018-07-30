using Adamant.Tools.Compiler.Bootstrap.Semantics.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Nodes.Expressions
{
    public abstract class Expression : SemanticNode
    {
        public DataType Type { get; }

        protected Expression(DataType type)
        {
            Type = type;
        }
    }
}
