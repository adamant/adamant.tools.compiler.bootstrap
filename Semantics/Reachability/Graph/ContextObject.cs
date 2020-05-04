using Adamant.Tools.Compiler.Bootstrap.AST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class ContextObject : HeapPlace
    {
        public ContextObject(IParameterSyntax parameter, Reference? originOfMutability)
            : base(parameter, originOfMutability)
        {
        }

        public ContextObject(IExpressionSyntax originSyntax, Reference? originOfMutability)
            : base(originSyntax, originOfMutability)
        {
        }
    }
}
