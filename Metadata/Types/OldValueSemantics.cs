using System.Diagnostics.CodeAnalysis;
using ExhaustiveMatching;

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
    /// Own - move an owned reference
    /// Borrow - copy the reference, borrow the object
    /// Share - copy the reference, share the object
    /// </summary>
    [SuppressMessage("Naming", "CA1717:Only FlagsAttribute enums should have plural names", Justification = "Name not plural")]
    public enum OldValueSemantics
    {
        /// <summary>
        /// Expression is acting as an lvalue not an rvalue
        /// </summary>
        LValue = -1,
        /// <summary>
        /// Expressions of type `never` and `void`, don't produce a value
        /// </summary>
        /// <remarks>The unknown</remarks>
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
        /// Copy a reference, share the referent
        /// </summary>
        Share,
    }

    public static class ValueSemanticsExtensions
    {
        public static string Action(this OldValueSemantics valueSemantics)
        {
            string mutability;
            switch (valueSemantics)
            {
                default:
                    throw ExhaustiveMatch.Failed(valueSemantics);
                case OldValueSemantics.LValue:
                    mutability = "deref";
                    break;
                case OldValueSemantics.Empty:
                    mutability = "void";
                    break;
                case OldValueSemantics.Move:
                    mutability = "move";
                    break;
                case OldValueSemantics.Copy:
                    mutability = "copy";
                    break;
                case OldValueSemantics.Own:
                    mutability = "own";
                    break;
                case OldValueSemantics.Borrow:
                    mutability = "borrow";
                    break;
                case OldValueSemantics.Share:
                    mutability = "share";
                    break;
            }

            return mutability;
        }
    }
}
