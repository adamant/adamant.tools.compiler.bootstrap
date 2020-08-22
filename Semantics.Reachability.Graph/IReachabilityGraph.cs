namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal interface IReachabilityGraph
    {
        void EnsureCurrentAccessIsUpToDate();
        void Drop(TempValue? temp);
        void Dirty();
        void Delete(Object obj);
        void LostReference(Object obj);
    }
}
