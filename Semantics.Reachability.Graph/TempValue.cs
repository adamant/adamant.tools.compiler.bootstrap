using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    public class TempValue : StackPlace
    {
        public ReferenceType ReferenceType { get; }

        private TempValue(ReferenceType referenceType)
        {
            ReferenceType = referenceType;
        }

        // TODO encapsulate this in the graph class
        public static TempValue? ForNewContextObject(IExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToNewContextObject(expression);
            var temp = new TempValue(referenceType);
            temp.AddReference(reference);
            return temp;
        }

        internal static TempValue? ForNewObject(INewObjectExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToNewObject(expression);
            var temp = new TempValue(referenceType);
            temp.AddReference(reference);
            return temp;
        }

        // TODO encapsulate this in the graph class
        public static TempValue? ForNewInvocationReturnedObject(IInvocationExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToNewInvocationReturnedObject(expression);
            var temp = new TempValue(referenceType);
            temp.AddReference(reference);
            return temp;
        }

        // TODO encapsulate this in the graph class
        public static TempValue? For(IExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            return new TempValue(referenceType);
        }

        public override string ToString()
        {
            return ": " + ReferenceType;
        }
    }
}
