using System;

namespace Adamant.Tools.Compiler.Bootstrap.Types
{
    /// <summary>
    /// The type of expressions and values whose type could not be determined or
    /// was somehow invalid. The unknown type can't be directly used in code.
    /// No well typed program contains any value of the unknown type.
    /// </summary>
    public sealed class UnknownType : DataType
    {
        #region Singleton
        internal static readonly UnknownType Instance = new UnknownType();

        private UnknownType() { }
        #endregion

        public override bool IsKnown => false;

        /// <summary>
        /// Like `never` values of type unknown can be assigned to any value.
        /// It acts like a bottom type in this respect.
        /// </summary>
        public override TypeSemantics Semantics => TypeSemantics.Never;

        public override string ToString()
        {
            return "⧼unknown⧽";
        }

        public override bool Equals(DataType? other)
        {
            // The unknown type is a singleton, so reference equality suffices
            return ReferenceEquals(this, other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(typeof(UnknownType));
        }
    }
}
