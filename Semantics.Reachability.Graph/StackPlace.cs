namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A stack place is a place that conceptually stored on the stack and is
    /// consequently a root for liveness. While temporary values may not
    /// actually be output to the stack, they conceptually stored on the stack
    /// because they have stack like storage.
    /// </summary>
    public abstract class StackPlace : MemoryPlace
    {
        private protected StackPlace(IReferenceGraph graph)
             : base(graph) { }
    }
}
