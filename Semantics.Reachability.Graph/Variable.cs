using System;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Types;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A parameter, local variable, or field of `self`
    /// </summary>
    public class Variable : StackPlace
    {
        public BindingSymbol Symbol { get; }

        /// <summary>
        /// The type of this variable or field. If the original type was optional
        /// this is the underlying reference type.
        /// </summary>
        public ReferenceType Type { get; }

        internal Variable(ReachabilityGraph graph, BindingSymbol symbol)
            : base(graph)
        {
            Symbol = symbol;
            Type = symbol.DataType.UnderlyingReferenceType()
                   ?? throw new ArgumentException("Must be a reference type", nameof(symbol));
        }

        internal static Variable? ForField(ReachabilityGraph graph, IFieldDeclaration field)
        {
            // Non-reference types don't participate in reachability (yet)
            var referenceType = field.Symbol.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            var variable = new Variable(graph, field.Symbol);
            variable.AddReference(Reference.ToNewFieldObject(graph, field));

            return variable;
        }

        internal static Variable? Declared(ReachabilityGraph graph, IVariableDeclarationStatement variable)
        {
            var referenceType = variable.Symbol.DataType.Known().UnderlyingReferenceType();
            if (referenceType is null) return null;

            return new Variable(graph, variable.Symbol);
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
            var mut = Symbol.IsMutableBinding ? "var " : "";
            var name = Symbol is SelfParameterSymbol ? "self" : Symbol.Name?.ToString();
            return $"{mut}{name}: {Symbol.DataType}";
        }
    }
}
