using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class TempValue : RootPlace
    {
        public ReferenceType ReferenceType { get; }

        private TempValue(ReferenceType referenceType)
        {
            ReferenceType = referenceType;
        }

        public static TempValue? ForNewContextObject(IExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToNewContextObject(expression);
            var temp = new TempValue(referenceType);
            temp.references.Add(reference);
            return temp;
        }

        public static TempValue? ForNewObject(INewObjectExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToNewObject(expression);
            var temp = new TempValue(referenceType);
            temp.references.Add(reference);
            return temp;
        }

        public static TempValue? ForNewInvocationReturnedObject(IInvocationExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var reference = Reference.ToNewInvocationReturnedObject(expression);
            var temp = new TempValue(referenceType);
            temp.references.Add(reference);
            return temp;
        }

        public static TempValue? For(IExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            return new TempValue(referenceType);
        }
    }
}
