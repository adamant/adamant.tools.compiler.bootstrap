using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    public interface IReference
    {
        Object Referent { get; }
        Ownership Ownership { get; }
        bool CouldHaveOwnership { get; }
        Access DeclaredAccess { get; }
        bool DeclaredReadable { get; }
        Phase Phase { get; }
        bool IsUsed { get; }
        bool IsReleased { get; }
        internal IEnumerable<IReference> Borrowers { get; }
        Access EffectiveAccess();
        bool IsUsedForBorrow();
        bool IsUsedForBorrowExceptBy(IReference reference);
        bool IsOriginFor(IReference reference);
        IReference Borrow();
        IReference Share();
        IReference Identify();
        void Use();
        internal void Release(IReferenceGraph graph);
    }
}
