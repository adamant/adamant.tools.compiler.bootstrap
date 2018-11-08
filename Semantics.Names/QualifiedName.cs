using System;
using System.Collections.Generic;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    /// <summary>
    /// A qualified name doesn't include the package. This is because the
    /// package name could be aliased.
    /// </summary>
    public class QualifiedName : Name, IEquatable<QualifiedName>
    {
        [NotNull] public Name Qualifier { get; }
        [NotNull] public override SimpleName UnqualifiedName { get; }
        //[NotNull] [ItemNotNull] public IEnumerable<SimpleName> FullName => Qualifier.Append(Name).AssertNotNull();

        public QualifiedName(
            [NotNull] Name qualifier,
            [NotNull] SimpleName simpleName)
        {
            Requires.NotNull(nameof(qualifier), qualifier);
            Requires.NotNull(nameof(simpleName), simpleName);
            Qualifier = qualifier;
            UnqualifiedName = simpleName;
        }

        [NotNull]
        public override string ToString()
        {
            return $"{Qualifier}.{UnqualifiedName}";
        }

        #region Equals
        public override bool Equals(object obj)
        {
            return Equals(obj as QualifiedName);
        }

        public bool Equals(QualifiedName other)
        {
            return other != null
                   && UnqualifiedName.Equals(other.UnqualifiedName)
                   && Qualifier.Equals(other.Qualifier);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Qualifier, UnqualifiedName);
        }

        public static bool operator ==(QualifiedName name1, QualifiedName name2)
        {
            return EqualityComparer<QualifiedName>.Default.Equals(name1, name2);
        }

        public static bool operator !=(QualifiedName name1, QualifiedName name2)
        {
            return !(name1 == name2);
        }
        #endregion

        public override bool IsDirectlyIn([NotNull] Name name)
        {
            Requires.NotNull(nameof(name), name);
            return Qualifier.Equals(name);
        }
    }
}
