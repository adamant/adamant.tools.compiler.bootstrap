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
    public class QualifiedName : Name
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
            Qualifier = qualifiers.ToList().AsReadOnly().AssertNotNull();
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
        public override string ToString()
        {
            return $"{string.Join('.', FullName)}";
        }
    }
}
