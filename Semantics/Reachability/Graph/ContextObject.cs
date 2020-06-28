using Adamant.Tools.Compiler.Bootstrap.AST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// Context objects represent objects whose lifetime is controlled outside
    /// the current function.
    /// </summary>
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
