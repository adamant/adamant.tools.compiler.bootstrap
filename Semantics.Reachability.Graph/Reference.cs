using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    public class Reference
    {
        public HeapPlace Referent { get; private set; }
        public Ownership Ownership { get; }
        public bool CouldHaveOwnership => Ownership == Ownership.Owns || Ownership == Ownership.PotentiallyOwns;

        /// <summary>
        /// The access the reference has based on the type it was created from
        /// </summary>
        public Access DeclaredAccess { get; }
        public bool DeclaredReadable => DeclaredAccess != Access.Identify;
        public Phase Phase { get; private set; } = Phase.Unused;
        public bool IsUsed => Phase == Phase.Used;
        private readonly List<Reference> borrowers = new List<Reference>();
        public IReadOnlyList<Reference> Borrowers { get; }

        private Reference(HeapPlace referent, Ownership ownership, Access declaredAccess)
        {
            Referent = referent;
            Ownership = ownership;
            DeclaredAccess = declaredAccess;
            Borrowers = borrowers.AsReadOnly();
        }

        private Reference(Ownership ownership, Access declaredAccess)
        {
            Referent = null!; // Referent must be set immediately after construction
            Ownership = ownership;
            DeclaredAccess = declaredAccess;
            Borrowers = borrowers.AsReadOnly();
        }

        // TODO encapsulate these in the graph class
        public static Reference ToNewParameterObject(IParameterSyntax parameter)
        {
            var referenceType = parameter.Type.Known().UnderlyingReferenceType()
                                ?? throw new ArgumentException("Must be a parameter with a reference type", nameof(parameter));

            var referenceCapability = referenceType.ReferenceCapability;
            var ownership = referenceCapability.ToOwnership();
            var access = referenceCapability.ToAccess();
            var reference = new Reference(ownership, access);
            var originOfMutability = access == Access.Mutable ? reference : null;
            reference.Referent = new Object(parameter, originOfMutability);
            return reference;
        }

        public static Reference ToNewParameterContextObject(IParameterSyntax parameter)
        {
            var referenceType = parameter.Type.Known().UnderlyingReferenceType()
                ?? throw new ArgumentException("Must be a parameter with a reference type",
                                                nameof(parameter));

            var referenceCapability = referenceType.ReferenceCapability;
            var ownership = referenceCapability.ToOwnership();
            var access = referenceCapability.ToAccess();
            var reference = new Reference(ownership, access);
            var originOfMutability = access == Access.Mutable ? reference : null;
            reference.Referent = new ContextObject(parameter, originOfMutability);
            return reference;
        }

        public static Reference ToNewContextObject(IExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType()
                                ?? throw new ArgumentException("Must be a parameter with a reference type",
                                    nameof(expression));

            var referenceCapability = referenceType.ReferenceCapability;
            var ownership = referenceCapability.ToOwnership();
            var access = referenceCapability.ToAccess();
            var reference = new Reference(ownership, access);
            var originOfMutability = access == Access.Mutable ? reference : null;
            reference.Referent = new ContextObject(expression, originOfMutability);
            return reference;
        }

        public static Reference ToNewObject(INewObjectExpressionSyntax expression)
        {
            return ToExpressionObject(expression);
        }

        public static Reference ToNewInvocationReturnedObject(IInvocationExpressionSyntax expression)
        {
            return ToExpressionObject(expression);
        }

        private static Reference ToExpressionObject(IExpressionSyntax expression)
        {
            var referenceType = expression.Type.Known().UnderlyingReferenceType()
                                ?? throw new ArgumentException("Must be a parameter with a reference type",
                                    nameof(expression));

            var referenceCapability = referenceType.ReferenceCapability;
            var ownership = referenceCapability.ToOwnership();
            var access = referenceCapability.ToAccess();
            var reference = new Reference(ownership, access);
            var originOfMutability = access == Access.Mutable ? reference : null;
            reference.Referent = new Object(expression, originOfMutability);
            return reference;
        }

        public static Reference ToNewFieldObject(IFieldDeclarationSyntax field)
        {
            var referenceType = field.Type.Known().UnderlyingReferenceType()
                                ?? throw new ArgumentException("Must be a parameter with a reference type",
                                    nameof(field));

            var referenceCapability = referenceType.ReferenceCapability;
            var ownership = referenceCapability.ToOwnership();
            var access = referenceCapability.ToAccess();
            var reference = new Reference(ownership, access);
            var originOfMutability = access == Access.Mutable ? reference : null;
            reference.Referent = new Object(field, originOfMutability);
            return reference;
        }

        /// <summary>
        /// The access it has at this momemnt given borrowing and sharing. Note that this
        /// doesn't account for shares.
        /// </summary>
        public Access EffectiveAccess()
        {
            switch (DeclaredAccess)
            {
                default:
                    throw ExhaustiveMatch.Failed(DeclaredAccess);
                case Access.Mutable:
                    // If we have been borrowed from, then this is temporarily an identity reference
                    // (unless there are shares in which case this would be read only)
                    if (IsUsedForBorrow())
                        return Access.Identify;

                    return Access.Mutable;
                case Access.ReadOnly:
                case Access.Identify:
                    // Access can't change from the declared access
                    return DeclaredAccess;
            }
        }

        public bool IsUsedForBorrow()
        {
            PruneBorrowers();
            return Borrowers.Any(b => b.IsUsedForBorrowInternal(null));
        }

        public bool IsUsedForBorrowExceptBy(Reference reference)
        {
            PruneBorrowers();
            return Borrowers.Any(b => b.IsUsedForBorrowInternal(reference));
        }

        private bool IsUsedForBorrowInternal(Reference? exceptBy)
        {
            return (Phase == Phase.Used && this != exceptBy)
                   || (Phase == Phase.Unused && Borrowers.Any(b => b.IsUsedForBorrowInternal(exceptBy)));
        }

        /// <summary>
        /// Prune out released borrowers, but watch out for reborrows
        /// </summary>
        private void PruneBorrowers()
        {
            // First prune recursively
            borrowers.ForEach(r => r.PruneBorrowers());

            var newBorrowers = new List<Reference>();
            // Remove released borrowers
            borrowers.RemoveAll(r =>
            {
                var released = r.Phase == Phase.Released;
                if (released)
                    // But grab their pruned list of borrowers
                    newBorrowers.AddRange(r.Borrowers);

                return released;
            });
            // And add it to ours instead
            borrowers.AddRange(newBorrowers);
        }

        public bool IsOriginFor(Reference reference)
        {
            PruneBorrowers();
            return IsOriginForInternal(reference);
        }

        private bool IsOriginForInternal(Reference reference)
        {
            return Borrowers.Any(b => b.Equals(reference))
                   || Borrowers.Any(b => b.IsOriginForInternal(reference));
        }

        public Reference Borrow()
        {
            if (DeclaredAccess != Access.Mutable)
                throw new InvalidOperationException("Can't borrow from non-mutable reference");

            var borrower = new Reference(Referent, Ownership.None, Access.Mutable);
            borrowers.Add(borrower);
            return borrower;
        }

        public Reference Share()
        {
            if (DeclaredAccess == Access.Identify)
                throw new InvalidOperationException("Can't share from identity reference");
            var share = new Reference(Referent, Ownership.None, Access.ReadOnly);
            return share;
        }

        public Reference Identify()
        {
            var identity = new Reference(Referent, Ownership.None, Access.Identify);
            return identity;
        }

        /// <summary>
        /// A reference is used when it is passed out of a function. Once that
        /// happens, we can't be sure how it will be accessed. Within the current
        /// function, references are just being moved around. Even if one is
        /// dereferenced, it doesn't have to be marked used, just checked to
        /// be usable at that moment.
        /// </summary>
        public void Use()
        {
            switch (Phase)
            {
                default:
                    throw ExhaustiveMatch.Failed(Phase);
                case Phase.Unused:
                    Phase = Phase.Used;
                    break;
                case Phase.Used:
                    // already used, do nothing
                    break;
                case Phase.Released:
                    throw new InvalidOperationException("Can't use released reference");
            }
        }

        /// <summary>
        /// Release this reference so that it no longer holds the referenced object
        /// </summary>
        internal void Release()
        {
            Phase = Phase.Released;
        }
    }
}
