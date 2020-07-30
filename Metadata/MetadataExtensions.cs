using System;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    public static class MetadataExtensions
    {
        private static readonly FixedList<IMetadata> UnknownMetadataList = new FixedList<IMetadata>(UnknownMetadata.Instance.Yield());

        public static bool IsGlobal(this IMetadata metadata)
        {
            return metadata.FullName is SimpleName;
        }

        public static FixedList<IMetadata> Lookup(this IMetadata metadata, SimpleName name)
        {
            if (metadata == UnknownMetadata.Instance) return UnknownMetadataList;
            return metadata switch
            {
                IParentMetadata parentMetadata =>
                    parentMetadata.ChildMetadata.TryGetValue(name, out var childMetadata)
                        ? childMetadata
                        : FixedList<IMetadata>.Empty,
                IBindingMetadata _ => FixedList<IMetadata>.Empty,
                _ => throw ExhaustiveMatch.Failed(metadata)
            };
        }

        public static T Assigned<T>(this T? metadata)
            where T : class, IMetadata
        {
            return metadata ?? throw new InvalidOperationException("Metadata not assigned");
        }
    }
}
