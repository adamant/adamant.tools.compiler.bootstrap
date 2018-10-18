using System;
using System.Collections.Generic;
using System.Linq;
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
        [NotNull] [ItemNotNull] public IReadOnlyList<SimpleName> Qualifier { get; }
        [NotNull] public SimpleName Name { get; }
        [NotNull] [ItemNotNull] public IEnumerable<SimpleName> FullName => Qualifier.Append(Name).AssertNotNull();

        public QualifiedName(
            [NotNull] [ItemNotNull] IEnumerable<SimpleName> qualifiers,
            [NotNull] SimpleName name)
        {
            Requires.NotNull(nameof(qualifiers), qualifiers);
            Requires.NotNull(nameof(name), name);
            Name = name;
            Qualifier = qualifiers.ToReadOnlyList();
        }

        public QualifiedName(
            [NotNull] SimpleName firstPart,
            [NotNull] [ItemNotNull] params SimpleName[] parts)
        {
            var allParts = firstPart.Yield().Concat(parts).ToList();
            var lastIndex = allParts.Count - 1;
            Name = allParts[lastIndex].AssertNotNull();
            allParts.RemoveAt(lastIndex);
            Qualifier = allParts.AsReadOnly().AssertNotNull();
        }

        [NotNull]
        public override QualifiedName Qualify([NotNull] SimpleName name)
        {
            Requires.NotNull(nameof(name), name);
            return new QualifiedName(FullName, name);
        }

        [NotNull]
        public override string ToString()
        {
            return $"{string.Join('.', FullName)}";
        }

        #region Equals
        public override bool Equals(object obj)
        {
            return Equals(obj as QualifiedName);
        }

        public bool Equals(QualifiedName other)
        {
            return other != null &&
                   EqualityComparer<SimpleName>.Default.Equals(Name, other.Name) &&
                   Qualifier.SequenceEqual(other.Qualifier);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Qualifier.Aggregate(Name.GetHashCode(), HashCode.Combine));
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
    }
}
