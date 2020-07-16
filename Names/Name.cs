using System;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    /// <summary>
    /// A name of a declaration. Note that names could even refer to local
    /// variables etc.
    /// </summary>
    [Closed(
        typeof(SimpleName),
        typeof(QualifiedName))]
    public abstract class Name : RootName, IEquatable<Name>
    {
        /// <summary>
        /// The unqualified (i.e. SimpleName) portion of the name
        /// </summary>

        public abstract SimpleName UnqualifiedName { get; }

        private protected Name() { }

        /// <summary>
        /// Construct a name from its segments
        /// </summary>

        public static Name From(
            string firstSegment,
            params string[] segments)
        {
            Name name = new SimpleName(firstSegment);
            foreach (var segment in segments)
                name = name.Qualify(segment);
            return name;
        }

        public override Name Qualify(Name name)
        {
            return name switch
            {
                SimpleName simpleName => new QualifiedName(this, simpleName),
                QualifiedName qualifiedName =>
                    new QualifiedName(Qualify(qualifiedName.Qualifier), qualifiedName.UnqualifiedName),
                _ => throw ExhaustiveMatch.Failed(name)
            };
        }

        public abstract bool HasQualifier(Name name);

        public abstract bool IsNestedIn(Name name);

        public abstract override string ToString();

        #region Equality
        public abstract bool Equals(Name? other);

        public sealed override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((Name)obj);
        }

        public abstract override int GetHashCode();

        public static bool operator ==(Name? name1, Name? name2)
        {
            return Equals(name1, name2);
        }

        public static bool operator !=(Name? name1, Name? name2)
        {
            return !(name1 == name2);
        }
        #endregion
    }
}
