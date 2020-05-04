using Adamant.Tools.Compiler.Bootstrap.AST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class Object : HeapPlace
    {
        public ISyntax OriginSyntax { get; }

        public Object(IParameterSyntax parameter, Reference? originOfMutability)
            : base(originOfMutability)
        {
            OriginSyntax = parameter;
        }

        public Object(IFieldDeclarationSyntax field, Reference? originOfMutability)
            : base(originOfMutability)
        {
            OriginSyntax = field;
        }

        public Object(IExpressionSyntax expression, Reference? originOfMutability)
            : base(originOfMutability)
        {
            OriginSyntax = expression;
        }
    }
}
