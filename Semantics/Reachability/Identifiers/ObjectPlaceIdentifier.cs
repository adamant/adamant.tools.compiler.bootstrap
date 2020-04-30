using Adamant.Tools.Compiler.Bootstrap.AST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Identifiers
{
    public class ObjectPlaceIdentifier : PlaceIdentifier
    {
        public ISyntax GeneratingSyntax { get; }

        public ObjectPlaceIdentifier(IExpressionSyntax expression)
        {
            GeneratingSyntax = expression;
        }

        public ObjectPlaceIdentifier(IParameterSyntax parameter)
        {
            GeneratingSyntax = parameter;
        }
    }
}
