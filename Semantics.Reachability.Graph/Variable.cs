using System;
using Adamant.Tools.Compiler.Bootstrap.CST;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A parameter, local variable, or field of `self`
    /// </summary>
    public class Variable : StackPlace
    {
        public IBindingMetadata Symbol { get; }

        /// <summary>
        /// The type of this variable or field. If the original type was optional
        /// this is the underlying reference type.
        /// </summary>
        public ReferenceType Type { get; }

        internal Variable(ReachabilityGraph graph, IBindingMetadata symbol)
            : base(graph)
        {
            Symbol = symbol;
            Type = symbol.Type.UnderlyingReferenceType()
                   ?? throw new ArgumentException("Must be a reference type", nameof(symbol));
        }

        internal static Variable? ForField(ReachabilityGraph graph, IFieldDeclarationSyntax field)
        {
            // Non-reference types don't participate in reachability (yet)
            var referenceType = field.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var variable = new Variable(graph, field);
            variable.AddReference(Reference.ToNewFieldObject(graph, field));

            return variable;
        }

        internal static Variable? Declared(ReachabilityGraph graph, IBindingMetadata symbol)
        {
            var referenceType = symbol.Type.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            return new Variable(graph, symbol);
        }

        internal void Assign(TempValue temp)
        {
            AddReferences(temp.StealReferences());
        }

        public void Dead()
        {
            ReleaseReferences();
        }

        public override string ToString()
        {
            return $"{Symbol.FullName.UnqualifiedName}: {Symbol.Type}";
        }
    }
}
