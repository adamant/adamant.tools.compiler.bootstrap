using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Scopes
{
    /// <summary>
    /// A lexical scope allows for the lookup of an <see cref="IMetadata"/> by a name
    /// </summary>
    public abstract class LexicalScope
    {
        private readonly MetadataSet metadata;
        private readonly MetadataSet nestedMetadata;

        protected LexicalScope(MetadataSet metadata, MetadataSet nestedMetadata)
        {
            this.metadata = metadata;
            this.nestedMetadata = nestedMetadata;
        }

        public abstract FixedList<IMetadata> LookupMetadataInGlobalScope(MaybeQualifiedName name);

        public virtual FixedList<IMetadata> LookupMetadata(SimpleName name, bool includeNested = true)
        {
            return metadata.TryGetValue(name, out var declaration)
                ? declaration
                : (includeNested && nestedMetadata.TryGetValue(name, out var nestedDeclaration) ? nestedDeclaration : FixedList<IMetadata>.Empty);
        }

        public FixedList<IMetadata> LookupMetadata(MaybeQualifiedName name, bool includeNested = true)
        {
            switch (name)
            {
                default:
                    throw ExhaustiveMatch.Failed(name);
                case SimpleName simpleName:
                    return LookupMetadata(simpleName, includeNested);
                case QualifiedName qualifiedName:
                    var containingMetadata = LookupMetadata(qualifiedName.Qualifier, includeNested);
                    return containingMetadata.OfType<IParentMetadata>()
                        .SelectMany(s => s.ChildMetadata[qualifiedName.UnqualifiedName])
                        .ToFixedList();
            }
        }
    }
}
