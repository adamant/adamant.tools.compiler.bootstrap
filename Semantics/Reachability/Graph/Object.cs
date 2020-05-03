using Adamant.Tools.Compiler.Bootstrap.AST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class Object : HeapPlace
    {
        public ISyntax OriginSyntax { get; }

        public Object(IParameterSyntax parameter)
        {
            OriginSyntax = parameter;
        }

        public Object(IExpressionSyntax expression)
        {
            OriginSyntax = expression;
        }
    }
}
