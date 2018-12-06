using Adamant.Tools.Compiler.Bootstrap.Framework;

namespace Adamant.Tools.Compiler.Bootstrap.Names
{
    /// <summary>
    /// A name of a declaration. Note that names could even refer to local
    /// variables etc.
    /// </summary>
    public abstract class Name : RootName
    {
        /// <summary>
        /// The unqualified (i.e. SimpleName) portion of the name
        /// </summary>

        public abstract SimpleName UnqualifiedName { get; }

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
            switch (name)
            {
                case SimpleName simpleName:
                    return new QualifiedName(this, simpleName);
                case QualifiedName qualifiedName:
                    return new QualifiedName(Qualify(qualifiedName.Qualifier), qualifiedName.UnqualifiedName);
                default:
                    throw NonExhaustiveMatchException.For(name);
            }
        }

        public abstract bool HasQualifier(Name name);

        public abstract override string ToString();
    }
}
