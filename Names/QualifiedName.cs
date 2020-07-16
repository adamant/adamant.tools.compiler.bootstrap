using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    /// <summary>
    /// A qualified name doesn't include the package. This is because the
    /// package name could be aliased.
    /// </summary>
    public sealed class QualifiedName : Name
    {
        public Name Qualifier { get; }
        public override SimpleName UnqualifiedName { get; }

        public QualifiedName(
            Name qualifier,
            SimpleName simpleName)
        {
            Qualifier = qualifier;
            UnqualifiedName = simpleName;
        }

        [DebuggerHidden]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public override IEnumerable<SimpleName> Segments => Qualifier.Segments.Append(UnqualifiedName);

        public override IEnumerable<Name> NestedInNames()
        {
            return Qualifier.NestedInNames().Append(Qualifier);
        }

        public override IEnumerable<Name> NamespaceNames()
        {
            return Qualifier.NamespaceNames().Append(this);
        }

        public override bool HasQualifier(Name name)
        {
            return Qualifier.Equals(name);
        }

        public override bool IsNestedIn(Name name)
        {
            return Qualifier.Equals(name) || Qualifier.IsNestedIn(name);
        }

        public override string ToString()
        {
            return $"{Qualifier}.{UnqualifiedName}";
        }

        #region Equals
        public override bool Equals(Name? other)
        {
            return other is QualifiedName name
                   && UnqualifiedName.Equals(name.UnqualifiedName)
                   && Qualifier.Equals(name.Qualifier);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Qualifier, UnqualifiedName);
        }
        #endregion
    }
}
