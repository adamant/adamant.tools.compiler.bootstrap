namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    /// <summary>
    /// A root place is a place that
    /// </summary>
    internal abstract class RootPlace : Place
    {
        //public void Owns(HeapPlace heapPlace, bool mutable)
        //{
        //    Reference.ToMovedParameter()
        //    references.Add(new Reference(heapPlace, Ownership.Owns, mutable ? Access.Mutable : Access.ReadOnly));
        //}

        //public void PotentiallyOwns(HeapPlace heapPlace, bool b)
        //{
        //    throw new System.NotImplementedException();
        //}

        //public void Borrows(HeapPlace heapPlace)
        //{
        //    references.Add(new Reference(heapPlace, None, Mutable));
        //}

        //public void Shares(HeapPlace heapPlace)
        //{
        //    references.Add(new Reference(heapPlace, None, ReadOnly));
        //}

        //public void Identifies(HeapPlace heapPlace)
        //{
        //    references.Add(new Reference(heapPlace, Ownership.None, Identity));
        //}
    }
}
