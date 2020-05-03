using Adamant.Tools.Compiler.Bootstrap.AST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class ContextObject : HeapPlace
    {
        public IParameterSyntax ForParameter { get; }

        public ContextObject(IParameterSyntax forParameter)
        {
            ForParameter = forParameter;
        }
    }
}
