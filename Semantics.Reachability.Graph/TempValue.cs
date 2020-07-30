using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    public class TempValue : StackPlace
    {
        private readonly IExpressionSyntax expression;
        public ReferenceType ReferenceType { get; }

        private TempValue(ReachabilityGraph graph, IExpressionSyntax expression, ReferenceType referenceType)
            : base(graph)
        {
            this.expression = expression;
            ReferenceType = referenceType;
        }

        // TODO encapsulate this in the graph class
        public static TempValue? ForNewContextObject(ReachabilityGraph graph, IExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToNewContextObject(graph, expression);
            var temp = new TempValue(graph, expression, referenceType);
            temp.AddReference(reference);
            return temp;
        }

        internal static TempValue? ForNewObject(ReachabilityGraph graph, INewObjectExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToNewObject(graph, expression);
            var temp = new TempValue(graph, expression, referenceType);
            temp.AddReference(reference);
            return temp;
        }

        // TODO encapsulate this in the graph class
        internal static TempValue? ForNewInvocationReturnedObject(ReachabilityGraph graph, IInvocationExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToNewInvocationReturnedObject(graph, expression);
            var temp = new TempValue(graph, expression, referenceType);
            temp.AddReference(reference);
            return temp;
        }

        internal static TempValue? ForFieldAccess(
            ReachabilityGraph graph,
            IFieldAccessExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToFieldAccess(graph, expression);
            var temp = new TempValue(graph, expression, referenceType);
            temp.AddReference(reference);
            return temp;
        }

        // TODO encapsulate this in the graph class
        public static TempValue? For(ReachabilityGraph graph, IExpressionSyntax expression, DataType? type = null)
        {
            type ??= expression.Type.Known();
            var referenceType = type.UnderlyingReferenceType();
            if (referenceType is null) return null;

            return new TempValue(graph, expression, referenceType);
        }

        public override string ToString()
        {
            return $"⟦{expression}⟧: {ReferenceType}";
        }
    }
}
