namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// The value semantics is how an expression produces its value.
    ///
    /// Value Types:
    /// Move - the value is moved
    /// Copy - the value is copied, either memcopy or using copy initializer
    /// 
    /// Reference Types:
    /// MoveOwner - move an owned reference
    /// Borrow - copy the reference, borrow the object
    /// Alias - copy the reference, alias the object
    /// </summary>
    public enum ValueSemantics
    {
        /// <summary>
        /// Expression is acting as an lvalue not an rvalue
        /// </summary>
        LValue = -1,
        /// <summary>
        /// Expressions of type `never` and `void`, don't produce a value
        /// </summary>
        Empty = 0,
        /// <summary>
        /// The value is moved
        /// </summary>
        Move = 1,
        /// <summary>
        /// The value is copied. For expression, does not indicate whether it is
        /// safe to bit copy the type or a copy function is needed
        /// </summary>
        Copy,
        /// <summary>
        /// The owning reference is moved
        /// </summary>
        Own,
        /// <summary>
        /// Copy a reference, borrow the referent
        /// </summary>
        Borrow,
        /// <summary>
        /// Copy a reference, alias the referent
        /// </summary>
        Alias,
    }
}
