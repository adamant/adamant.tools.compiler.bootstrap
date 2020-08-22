using System;
using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.AST;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Reachability.Graph
{
    internal class Reference : IReference
    {
        public Object Referent { get; private set; }
        public Ownership Ownership { get; }
        public bool CouldHaveOwnership => Ownership == Ownership.Owns || Ownership == Ownership.PotentiallyOwns;

        /// <summary>
        /// The access the reference has based on the type it was created from
        /// </summary>
        public Access DeclaredAccess { get; }
        public bool DeclaredReadable => DeclaredAccess != Access.Identify;
        public Phase Phase { get; private set; } = Phase.Unused;
        public bool IsUsed => Phase == Phase.Used;
        public bool IsReleased => Phase == Phase.Released;
        private readonly List<Reference> borrowers = new List<Reference>();
        internal IReadOnlyList<Reference> Borrowers { get; }
        IEnumerable<IReference> IReference.Borrowers => Borrowers;

        private Reference(Object referent, Ownership ownership, Access declaredAccess)
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

        internal static Reference ToNewParameterObject(ReachabilityGraph graph, IBindingParameter parameter)
        {
            var referenceType = parameter.Symbol.DataType.Known().UnderlyingReferenceType()
                                ?? throw new ArgumentException("Must be a parameter with a reference type", nameof(parameter));

            var referenceCapability = referenceType.ReferenceCapability;
            var ownership = referenceCapability.ToOwnership();
            var access = referenceCapability.ToAccess();
            var reference = new Reference(ownership, access);
            var originOfMutability = access == Access.Mutable ? reference : null;
            reference.Referent = new Object(graph, false, parameter, originOfMutability);
            return reference;
        }

        internal static Reference ToNewParameterContextObject(ReachabilityGraph graph, IBindingParameter parameter)
        {
            var referenceType = parameter.Symbol.DataType.Known().UnderlyingReferenceType()
                ?? throw new ArgumentException("Must be a parameter with a reference type",
                                                nameof(parameter));

            var referenceCapability = referenceType.ReferenceCapability;
            var ownership = referenceCapability.ToOwnership();
            var access = referenceCapability.ToAccess();
            var reference = new Reference(ownership, access);
            var originOfMutability = access == Access.Mutable ? reference : null;
            reference.Referent = new Object(graph, true, parameter, originOfMutability);
            return reference;
        }

        internal static Reference ToNewContextObject(ReachabilityGraph graph, IExpression expression)
        {
            var referenceType = expression.DataType.Known().UnderlyingReferenceType()
                                ?? throw new ArgumentException("Must be a parameter with a reference type",
                                    nameof(expression));

            var referenceCapability = referenceType.ReferenceCapability;
            var ownership = referenceCapability.ToOwnership();
            var access = referenceCapability.ToAccess();
            var reference = new Reference(ownership, access);
            var originOfMutability = access == Access.Mutable ? reference : null;
            reference.Referent = new Object(graph, true, expression, originOfMutability);
            return reference;
        }

        internal static Reference ToNewObject(ReachabilityGraph graph, INewObjectExpression expression)
        {
            return ToExpressionObject(graph, expression);
        }

        internal static Reference ToNewInvocationReturnedObject(ReachabilityGraph graph, IInvocationExpression expression)
        {
            return ToExpressionObject(graph, expression);
        }

        internal static Reference ToFieldAccess(ReachabilityGraph graph, IFieldAccessExpression expression)
        {
            return ToExpressionObject(graph, expression);
        }

        private static Reference ToExpressionObject(ReachabilityGraph graph, IExpression expression)
        {
            var referenceType = expression.DataType.Known().UnderlyingReferenceType()
                                ?? throw new ArgumentException("Must be a parameter with a reference type",
                                    nameof(expression));

            var referenceCapability = referenceType.ReferenceCapability;
            var ownership = referenceCapability.ToOwnership();
            var access = referenceCapability.ToAccess();
            var reference = new Reference(ownership, access);
            var originOfMutability = access == Access.Mutable ? reference : null;
            reference.Referent = new Object(graph, false, expression, originOfMutability);
            return reference;
        }

        public static Reference ToNewFieldObject(ReachabilityGraph graph, IFieldDeclaration field)
        {
            var referenceType = field.Symbol.DataType.Known().UnderlyingReferenceType()
                                ?? throw new ArgumentException("Must be a parameter with a reference type",
                                    nameof(field));

            var referenceCapability = referenceType.ReferenceCapability;
            var ownership = referenceCapability.ToOwnership();
            var access = referenceCapability.ToAccess();
            var reference = new Reference(ownership, access);
            var originOfMutability = access == Access.Mutable ? reference : null;
            reference.Referent = new Object(graph, field, originOfMutability);
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

        public bool IsUsedForBorrowExceptBy(IReference reference)
        {
            PruneBorrowers();
            return Borrowers.Any(b => b.IsUsedForBorrowInternal(reference));
        }

        private bool IsUsedForBorrowInternal(IReference? exceptBy)
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

        public bool IsOriginFor(IReference reference)
        {
            PruneBorrowers();
            return IsOriginForInternal(reference);
        }

        private bool IsOriginForInternal(IReference reference)
        {
            return reference == this
                || Borrowers.Any(b => b.Equals(reference))
                || Borrowers.Any(b => b.IsOriginForInternal(reference));
        }

        public IReference Borrow()
        {
            if (DeclaredAccess != Access.Mutable)
                throw new InvalidOperationException("Can't borrow from non-mutable reference");

            var borrower = new Reference(Referent, Ownership.None, Access.Mutable);
            borrowers.Add(borrower);
            return borrower;
        }

        public IReference Share()
        {
            if (DeclaredAccess == Access.Identify)
                throw new InvalidOperationException("Can't share from identity reference");
            var share = new Reference(Referent, Ownership.None, Access.ReadOnly);
            return share;
        }

        public IReference Identify()
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
        public void Use(ReachabilityGraph graph)
        {
            switch (Phase)
            {
                default:
                    throw ExhaustiveMatch.Failed(Phase);
                case Phase.Unused:
                    Phase = Phase.Used;
                    graph.Dirty();
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
        void IReference.Release(IReachabilityGraph graph)
        {
            Phase = Phase.Released;
            graph.Dirty();
        }
    }
}
