using Adamant.Tools.Compiler.Bootstrap.AST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class Object : HeapPlace
    {
        public Object(IParameterSyntax parameter, Reference? originOfMutability)
            : base(parameter, originOfMutability)
        {
        }

        public Object(IFieldDeclarationSyntax field, Reference? originOfMutability)
            : base(field, originOfMutability)
        {
        }

        public Object(IExpressionSyntax expression, Reference? originOfMutability)
            : base(expression, originOfMutability)
        {
        }
    }
}
