namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// The value semantics is how an expression produces its value. That is
    /// whether a move or a copy. For reference types the value mode applies to the
    /// reference itself, not to the referent.
    ///
    /// Reference Types:
    /// Move - an owned reference is being moved
    /// Copy - a reference is copied, i.e. borrowed
    ///
    /// Value Types:
    /// Move - the value is moved
    /// Copy - the value is copied, either memcopy or using copy initializer
    /// </summary>
    public enum ValueSemantics
    {
        /// <summary>
        /// This value type is a move type
        /// </summary>
        Move = 1,
        /// <summary>
        /// This value type is a copy type. Does not indicate whether it is safe
        /// to bit copy the type or a copy function is needed
        /// </summary>
        Copy,
        /// <summary>
        /// Expression of type `never`, don't return
        /// </summary>
        Never,
    }
}
