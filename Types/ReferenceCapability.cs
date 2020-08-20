namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    public enum ReferenceCapability
    {
        /// <summary>
        /// A reference that owns the object and there *may* be references from
        /// the subtree out to other non-constant objects. Those references
        /// place an upper bound on the lifetime of the object referenced.
        /// </summary>
        Owned,
        /// <summary>
        /// An owned reference that is also mutable.
        /// </summary>
        OwnedMutable,
        /// <summary>
        /// A reference that owns the object and there are *no* references from
        /// the subtree out to other non-constant objects. As a result, there
        /// is no upper bound on the lifetime of the object referenced.
        /// </summary>
        Isolated,
        /// <summary>
        /// An isolated reference that is also mutable.
        /// </summary>
        IsolatedMutable,
        /// <summary>
        /// A reference that may or may not own the referenced object based on
        /// a flag carried with the reference.
        /// </summary>
        Held,
        /// <summary>
        /// A held reference that is also mutable.
        /// </summary>
        HeldMutable,
        /// <summary>
        /// A reference that shares access to an object with multiple other
        /// references. As a consequence, the object is frozen for the lifetime
        /// of the references.
        /// </summary>
        Shared,
        /// <summary>
        /// A reference that has exclusive access to an object but not ownership.
        /// As a consequence, the object can be mutated through this reference.
        /// </summary>
        Borrowed,
        /// <summary>
        /// A reference that can be used to identify an object but not read or
        /// write to it.
        /// </summary>
        Identity,
    }
}
