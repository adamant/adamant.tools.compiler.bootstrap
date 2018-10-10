using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using JetBrains.Annotations;

namespace Adamant.Tools.Compiler.Bootstrap.Semantics.Names
{
    public class FullyQualifiedName
    {
        [NotNull] public string Package { get; }
        [NotNull] [ItemNotNull] public IReadOnlyList<string> Qualifiers { get; }
        [NotNull] public string Name { get; }
        [NotNull] [ItemNotNull] public IEnumerable<string> FullName => Qualifiers.Append(Name).AssertNotNull();

        public FullyQualifiedName(
            [NotNull] string package,
            [NotNull] [ItemNotNull] IEnumerable<string> qualifiers,
            [NotNull] string name)
        {
            Requires.NotNull(nameof(package), package);
            Requires.NotNull(nameof(qualifiers), qualifiers);
            Requires.NotNull(nameof(name), name);
            Package = package;
            Name = name;
            Qualifiers = qualifiers.ToList().AsReadOnly().AssertNotNull();
        }

        public override string ToString()
        {
            return $"{Package}::{string.Join('.', FullName)}";
        }
    }
}
