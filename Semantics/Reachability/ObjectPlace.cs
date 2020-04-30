using Adamant.Tools.Compiler.Bootstrap.AST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability
{
    public class ObjectPlace : Place
    {
        private readonly ISyntax syntax;

        public ObjectPlace(IExpressionSyntax expression)
        {
            syntax = expression;
        }

        public ObjectPlace(IParameterSyntax parameter)
        {
            syntax = parameter;
        }
    }
}
