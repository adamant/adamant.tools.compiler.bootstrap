using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Tokens;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    /// <summary>
    /// A standard name
    /// </summary>
    public sealed class Name : TypeName
    {
        private static readonly Regex NeedsQuoted = new Regex(@"[\\ #ₛ]", RegexOptions.Compiled);

        public Name(string text)
            : base(text) { }

        public override bool Equals(TypeName? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return other is Name otherName
                && Text == otherName.Text;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(typeof(Name), Text);
        }

        public override string ToString()
        {
            if (TokenTypes.Keywords.Contains(Text)
               ||TokenTypes.ReservedWords.Contains(Text))
                return '\\' + Text;

            var text = Text.Escape();
            if (NeedsQuoted.IsMatch(text)) text += $@"\""{text}""";
            return text;
        }

        public override SimpleName ToSimpleName()
        {
            return new SimpleName(Text);
        }

        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates",
            Justification = "Constructor is alternative")]
        public static implicit operator Name(string text)
        {
            return new Name(text);
        }
    }
}
