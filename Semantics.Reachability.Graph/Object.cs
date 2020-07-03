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

        internal Object(ReachabilityGraph graph, bool isContext, IParameterSyntax parameter, Reference? originOfMutability)
            : base(graph, parameter, originOfMutability)
        {
            IsContext = isContext;
        }

        internal Object(ReachabilityGraph graph, bool isContext, IExpressionSyntax originSyntax, Reference? originOfMutability)
            : base(graph, originSyntax, originOfMutability)
        {
            IsContext = isContext;
        }

        internal Object(ReachabilityGraph graph, IFieldDeclarationSyntax field, Reference? originOfMutability)
            : base(graph, field, originOfMutability)
        {
            IsContext = false;
        }
    }
}
