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
        public SpecialTypeName(string text)
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
    }
}
