using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    public class TempValue : StackPlace
    {
        public ReferenceType ReferenceType { get; }

        private TempValue(ReachabilityGraph graph, ReferenceType referenceType)
            : base(graph)
        {
            ReferenceType = referenceType;
        }

        // TODO encapsulate this in the graph class
        public static TempValue? ForNewContextObject(ReachabilityGraph graph, IExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToNewContextObject(graph, expression);
            var temp = new TempValue(graph, referenceType);
            temp.AddReference(reference);
            return temp;
        }

        internal static TempValue? ForNewObject(ReachabilityGraph graph, INewObjectExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToNewObject(graph, expression);
            var temp = new TempValue(graph, referenceType);
            temp.AddReference(reference);
            return temp;
        }

        // TODO encapsulate this in the graph class
        public static TempValue? ForNewInvocationReturnedObject(ReachabilityGraph graph, IInvocationExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToNewInvocationReturnedObject(graph, expression);
            var temp = new TempValue(graph, referenceType);
            temp.AddReference(reference);
            return temp;
        }

        // TODO encapsulate this in the graph class
        public static TempValue? For(ReachabilityGraph graph, IExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            return new TempValue(graph, referenceType);
        }

        public override string ToString()
        {
            return "⟦ ⟧: " + ReferenceType;
        }
    }
}
