using Adamant.Tools.Compiler.Bootstrap.AST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class ObjectPlace : Place
    {
        public ISyntax OriginSyntax { get; }

        public ObjectPlace(IParameterSyntax parameter)
        {
            OriginSyntax = parameter;
        }

        public ObjectPlace(IExpressionSyntax expression)
        {
            OriginSyntax = expression;
        }
    }
}
