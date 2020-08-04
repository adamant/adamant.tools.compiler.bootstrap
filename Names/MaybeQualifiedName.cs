using System;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    // TODO It seems like the idea of qualified names doesn't make sense. That is something for symbols
    /// <summary>
    /// A name of a declaration. Note that names could even refer to local
    /// variables etc.
    /// </summary>
    [Closed(
        typeof(SimpleName),
        typeof(QualifiedName))]
    public abstract class MaybeQualifiedName : RootName, IEquatable<MaybeQualifiedName>
    {
        /// <summary>
        /// The unqualified (i.e. SimpleName) portion of the name
        /// </summary>

        public abstract SimpleName UnqualifiedName { get; }

        private protected MaybeQualifiedName() { }

        /// <summary>
        /// Construct a name from its segments
        /// </summary>

        public static MaybeQualifiedName From(
            string firstSegment,
            params string[] segments)
        {
            MaybeQualifiedName name = new SimpleName(firstSegment);
            foreach (var segment in segments)
                name = name.Qualify(segment);
            return name;
        }

        public override MaybeQualifiedName Qualify(MaybeQualifiedName name)
        {
            return name switch
            {
                SimpleName simpleName => new QualifiedName(this, simpleName),
                QualifiedName qualifiedName =>
                    new QualifiedName(Qualify(qualifiedName.Qualifier), qualifiedName.UnqualifiedName),
                _ => throw ExhaustiveMatch.Failed(name)
            };
        }

        public abstract bool HasQualifier(MaybeQualifiedName name);

        public abstract bool IsNestedIn(MaybeQualifiedName name);

        public abstract override string ToString();

        #region Equality
        public abstract bool Equals(MaybeQualifiedName? other);

        public sealed override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == GetType() && Equals((MaybeQualifiedName)obj);
        }

        public abstract override int GetHashCode();

        public static bool operator ==(MaybeQualifiedName? name1, MaybeQualifiedName? name2)
        {
            return Equals(name1, name2);
        }

        public static bool operator !=(MaybeQualifiedName? name1, MaybeQualifiedName? name2)
        {
            return !(name1 == name2);
        }
        #endregion
    }
}
