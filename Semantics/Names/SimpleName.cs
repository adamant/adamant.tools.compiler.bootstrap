using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class SimpleName : Name, IEquatable<SimpleName>
    {
        [NotNull] public readonly string Text;
        [NotNull] public readonly bool IsSpecial;

        public SimpleName([NotNull] string text)
        {
            Requires.NotNull(nameof(text), text);
            Text = text;
            IsSpecial = false;
        }

        public SimpleName([NotNull] string text, bool isSpecial)
        {
            Requires.NotNull(nameof(text), text);
            Text = text;
            IsSpecial = isSpecial;
        }

        public override string ToString()
        {
            // TODO deal with IsSpecial and Special chars like space
            return Text;
        }

        [NotNull]
        public override QualifiedName Qualify([NotNull] SimpleName name)
        {
            Requires.NotNull(nameof(name), name);
            return new QualifiedName(this, name);
        }

        #region Equals
        public override bool Equals(object obj)
        {
            return Equals(obj as SimpleName);
        }

        public bool Equals(SimpleName other)
        {
            return other != null &&
                   Text == other.Text &&
                   IsSpecial == other.IsSpecial;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, IsSpecial);
        }

        public static bool operator ==(SimpleName name1, SimpleName name2)
        {
            return EqualityComparer<SimpleName>.Default.Equals(name1, name2);
        }

        public static bool operator !=(SimpleName name1, SimpleName name2)
        {
            return !(name1 == name2);
        }

        public static implicit operator SimpleName([NotNull] string text)
        {
            return new SimpleName(text);
        }
        #endregion
    }
}
