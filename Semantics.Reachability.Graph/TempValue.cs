using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    public class TempValue : StackPlace
    {
        private readonly IExpression expression;
        public ReferenceType ReferenceType { get; }

        private TempValue(ReachabilityGraph graph, IExpression expression, ReferenceType referenceType)
            : base(graph)
        {
            this.expression = expression;
            ReferenceType = referenceType;
        }

        internal static TempValue? ForNewContextObject(ReachabilityGraph graph, IExpression expression)
        {
            var referenceType = expression.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToNewContextObject(graph, expression);
            var temp = new TempValue(graph, expression, referenceType);
            temp.AddReference(reference);
            return temp;
        }

        internal static TempValue? ForNewObject(ReachabilityGraph graph, INewObjectExpression expression)
        {
            var referenceType = expression.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToNewObject(graph, expression);
            var temp = new TempValue(graph, expression, referenceType);
            temp.AddReference(reference);
            return temp;
        }

        internal static TempValue? ForNewInvocationReturnedObject(ReachabilityGraph graph, IInvocationExpression expression)
        {
            var referenceType = expression.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToNewInvocationReturnedObject(graph, expression);
            var temp = new TempValue(graph, expression, referenceType);
            temp.AddReference(reference);
            return temp;
        }

        internal static TempValue? ForFieldAccess(
            ReachabilityGraph graph,
            IFieldAccessExpression expression)
        {
            var referenceType = expression.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToFieldAccess(graph, expression);
            var temp = new TempValue(graph, expression, referenceType);
            temp.AddReference(reference);
            return temp;
        }

        internal static TempValue? For(ReachabilityGraph graph, IExpression expression, DataType? type = null)
        {
            type ??= expression.DataType.Known();
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
