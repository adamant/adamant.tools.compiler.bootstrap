using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.AST;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// Context objects represent objects whose lifetime is controlled outside
    /// the current function.
    /// </summary>
    [SuppressMessage("Naming", "CA1720:Identifier contains type name", Justification = "<Pending>")]
    public class Object : HeapPlace
    {
        public bool IsContext { get; }

        internal Object(IReferenceGraph graph, bool isContext, IAbstractSyntax syntax, Reference? originOfMutability)
            : base(graph, syntax, originOfMutability)
        {
            IsContext = isContext;
        }
    }
}
