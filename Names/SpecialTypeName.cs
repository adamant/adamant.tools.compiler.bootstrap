using System;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    /// <summary>
    /// A special type name is the name of one of the special types in the system.
    /// They are distinguished because for example `bool` is a special name, but
    /// `\"bool"` is a regular name with the same text.
    /// </summary>
    public sealed class SpecialTypeName : TypeName
    {
        #region Empty and Simple Types
        public static readonly SpecialTypeName Void = new SpecialTypeName("void");
        public static readonly SpecialTypeName Never = new SpecialTypeName("never");
        public static readonly SpecialTypeName Bool = new SpecialTypeName("bool");
        public static readonly SpecialTypeName Any = new SpecialTypeName("Any");
        public static readonly SpecialTypeName Byte = new SpecialTypeName("byte");
#pragma warning disable CA1720
        public static readonly SpecialTypeName Int = new SpecialTypeName("int");
        public static readonly SpecialTypeName UInt = new SpecialTypeName("uint");
#pragma warning restore CA1720
        public static readonly SpecialTypeName Size = new SpecialTypeName("size");
        public static readonly SpecialTypeName Offset = new SpecialTypeName("offset");
        #endregion

        #region Constant Types
        public static readonly SpecialTypeName True = new SpecialTypeName("True");
        public static readonly SpecialTypeName False = new SpecialTypeName("False");
        public static readonly SpecialTypeName ConstInt = new SpecialTypeName("ConstInt");
        #endregion

        private SpecialTypeName(string text)
            : base(text) { }



        public override bool Equals(TypeName? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is SpecialTypeName otherName && Text == otherName.Text;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(typeof(SpecialTypeName), Text);
        }

        public override string ToString()
        {
            return Text;
        }

        public override SimpleName ToSimpleName()
        {
            return SimpleName.Special(Text);
        }
    }
}
