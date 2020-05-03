using System.Collections.Generic;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class Reference
    {
        public HeapPlace Referent { get; }
        public Ownership Ownership { get; }
        public Access Access { get; }
        private List<Reference> borrowers = new List<Reference>();
        public IReadOnlyList<Reference> Borrowers { get; }

        public Reference(HeapPlace referent, Ownership ownership, Access access)
        {
            Referent = referent;
            Ownership = ownership;
            Access = access;
            Borrowers = borrowers.AsReadOnly();
        }

        public Reference Borrow()
        {
            var borrower = new Reference(Referent, Ownership.None, Access.Mutable);
            borrowers.Add(borrower);
            return borrower;
        }
    }
}
