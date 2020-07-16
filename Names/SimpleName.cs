using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    public sealed class SimpleName : Name
    {
        private static readonly Regex NeedsQuoted = new Regex(@"[\\ #ₛ]", RegexOptions.Compiled);

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override SimpleName UnqualifiedName => this;
        public string Text { get; }
        /// <summary>
        /// Since the same variable name can be declared more than once, they are
        /// given declaration numbers. The first one is declaration 0 and is
        /// displayed without a declaration number.
        /// </summary>
        public int? DeclarationNumber { get; }
        public bool IsSpecial { get; }

        public SimpleName(string text)
            : this(text, false, null)
        {
        }

        public static SimpleName Variable(string text, int number)
        {
            if (number <0) throw new ArgumentOutOfRangeException(nameof(number), "Must not be negative");
            return new SimpleName(text, false, number);
        }

        public static SimpleName Special(string text)
        {
            return new SimpleName(text, true, null);
        }

        private SimpleName(string text, bool isSpecial, int? declarationNumber)
        {
            Text = text;
            IsSpecial = isSpecial;
            DeclarationNumber = declarationNumber;
        }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override IEnumerable<SimpleName> Segments => this.Yield();

        public override IEnumerable<Name> NestedInNames()
        {
            return Enumerable.Empty<Name>();
        }

        public override IEnumerable<Name> NamespaceNames()
        {
            return this.Yield();
        }

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

        public SimpleName WithoutDeclarationNumber()
        {
            if (DeclarationNumber is null)
                return this;
            return new SimpleName(Text, IsSpecial, null);
        }

        public override string ToString()
        {
            var escapedName = Text.Escape();
            if (NeedsQuoted.IsMatch(escapedName))
                escapedName += $@"""{escapedName}""";
            if (DeclarationNumber != null && DeclarationNumber != 0)
                escapedName += "#" + DeclarationNumber;
            return escapedName;
        }

        #region Equals
        public override bool Equals(Name? other)
        {
            return other is SimpleName name
                && Text == name.Text
                && IsSpecial == name.IsSpecial
                && DeclarationNumber == name.DeclarationNumber;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, IsSpecial, DeclarationNumber);
        }
        #endregion
    }
}
