using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    public sealed class SimpleName : Name, IEquatable<SimpleName>
    {
        private static readonly Regex NeedsQuoted = new Regex(@"[\\ #ₛ]", RegexOptions.Compiled);

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override SimpleName UnqualifiedName => this;
        public readonly string Text;
        public readonly int? Number;
        public readonly bool IsSpecial;

        public SimpleName(string text)
            : this(text, false, null)
        {
        }

        public static SimpleName Variable(string text, int number)
        {
            return new SimpleName(text, false, number);
        }

        public static SimpleName Special(string text)
        {
            return new SimpleName(text, true, null);
        }

        private SimpleName(string text, bool isSpecial, int? number)
        {
            Text = text;
            IsSpecial = isSpecial;
            Number = number;
        }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override IEnumerable<SimpleName> Segments => this.Yield();

        public override bool HasQualifier(Name name)
        {
            // A simple name doesn't have a qualifier
            return false;
        }

        public override bool IsNestedIn(Name name)
        {
            // A simple name isn't nested in any other names
            return false;
        }

        public SimpleName WithoutNumber()
        {
            if (Number == null)
                return this;
            return new SimpleName(Text, IsSpecial, null);
        }

        public override string ToString()
        {
            var escapedName = Text.Escape();
            if (NeedsQuoted.IsMatch(escapedName))
                escapedName += $@"""{escapedName}""";
            if (Number != null && Number != 0)
                escapedName += "#" + Number;
            return escapedName;
        }

        #region Equals
        public override bool Equals(object obj)
        {
            return Equals(obj as SimpleName);
        }

        public bool Equals(SimpleName other)
        {
            return other != null
                && Text == other.Text
                && IsSpecial == other.IsSpecial
                && Number == other.Number;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, IsSpecial, Number);
        }

        public static bool operator ==(SimpleName name1, SimpleName name2)
        {
            return EqualityComparer<SimpleName>.Default.Equals(name1, name2);
        }

        public static bool operator !=(SimpleName name1, SimpleName name2)
        {
            return !(name1 == name2);
        }
        #endregion
    }
}
