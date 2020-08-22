using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    public class TempValue : StackPlace
    {
        private readonly IExpression expression;
        public ReferenceType ReferenceType { get; }

        internal TempValue(IReferenceGraph graph, IExpression expression, ReferenceType referenceType)
            : base(graph)
        {
            this.expression = expression;
            ReferenceType = referenceType;
        }

        public override string ToString()
        {
            return $"⟦{expression}⟧: {ReferenceType}";
        }
    }
}
