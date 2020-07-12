using System.Diagnostics.CodeAnalysis;

namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    /// <summary>
    /// The semantics of the value of an expression
    ///
    /// Move - the value is moved
    /// Copy - the value is copied, either memcopy or using copy initializer
    /// Acquire - take ownership from (can apply to isolated, owned, and held)
    /// Borrow - copy the reference, borrow the object
    /// Share - copy the reference, share the object
    /// </summary>
    [SuppressMessage("Naming", "CA1717:Only FlagsAttribute enums should have plural names", Justification = "Name not plural")]
    public enum ExpressionSemantics
    {
        /// <summary>
        /// Never returns or has unknown return
        /// </summary>
        Never = -1,
        /// <summary>
        /// Expressions of type `void`, don't produce a value
        /// </summary>
        Void = 0,
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
        /// The ownership is transferred between the references. Leaves the
        /// giver in a moved state.
        /// </summary>
        Acquire,
        /// <summary>
        /// Copy a reference, borrow the referent
        /// </summary>
        Borrow,
        /// <summary>
        /// Copy a reference, share the referent
        /// </summary>
        Share,
        /// <summary>
        /// Take a reference to a place. Used for LValues
        /// </summary>
        CreateReference,
    }
}
