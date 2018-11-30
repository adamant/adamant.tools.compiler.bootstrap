using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    public class SimpleName : Name, IEquatable<SimpleName>
    {
        [NotNull] private static readonly Regex NeedsQuoted = new Regex(@"[\\ #ₛ]", RegexOptions.Compiled);

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull] public override SimpleName UnqualifiedName => this;
        [NotNull] public readonly string Text;
        public readonly int? Number;
        [NotNull] public readonly bool IsSpecial;

        public SimpleName([NotNull] string text)
            : this(text, false, null)
        {
        }

        [NotNull]
        public static SimpleName Variable([NotNull] string text, int number)
        {
            return new SimpleName(text, false, number);
        }

        [NotNull]
        public static SimpleName Special([NotNull] string text)
        {
            return new SimpleName(text, true, null);
        }

        private SimpleName([NotNull] string text, bool isSpecial, int? number)
        {
            Requires.NotNull(nameof(text), text);
            Text = text;
            IsSpecial = isSpecial;
            Number = number;
        }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        [NotNull] public override IEnumerable<SimpleName> Segments => this.Yield();

        public override bool HasQualifier([NotNull] Name name)
        {
            Requires.NotNull(nameof(name), name);
            // A simple name doesn't have a qualifier
            return false;
        }

        [NotNull]
        public SimpleName WithoutNumber()
        {
            if (Number == null) return this;
            return new SimpleName(Text, IsSpecial, null);
        }

        public override string ToString()
        {
            var escapedName = Text.Escape();
            if (NeedsQuoted.IsMatch(escapedName))
                escapedName += $@"""{escapedName}""";
            if (Number != null) escapedName += "#" + Number;
            if (IsSpecial) escapedName = "ₛ" + escapedName;
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
