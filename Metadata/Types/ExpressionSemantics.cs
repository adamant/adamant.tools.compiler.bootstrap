using System.Diagnostics.CodeAnalysis;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Types
{
    /// <summary>
    /// The semantics of the value of an expression
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
    // TODO remove from types
    // TODO move to AST
    // TODO add to all expressions
    public enum ExpressionSemantics
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
        public static string Action(this ExpressionSemantics valueSemantics)
        {
            string mutability;
            switch (valueSemantics)
            {
                default:
                    throw ExhaustiveMatch.Failed(valueSemantics);
                case ExpressionSemantics.LValue:
                    mutability = "deref";
                    break;
                case ExpressionSemantics.Empty:
                    mutability = "void";
                    break;
                case ExpressionSemantics.Move:
                    mutability = "move";
                    break;
                case ExpressionSemantics.Copy:
                    mutability = "copy";
                    break;
                case ExpressionSemantics.Own:
                    mutability = "own";
                    break;
                case ExpressionSemantics.Borrow:
                    mutability = "borrow";
                    break;
                case ExpressionSemantics.Share:
                    mutability = "share";
                    break;
            }

            return mutability;
        }
    }
}
