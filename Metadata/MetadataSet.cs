using System.Collections.Generic;
using System.Linq;
using Adamant.Tools.Compiler.Bootstrap.Framework;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    /// <summary>
    /// An immutable set of metadata that is indexed by their unqualified name
    /// </summary>
    public class MetadataSet : FixedDictionary<SimpleName, FixedList<IMetadata>>, IEnumerable<IMetadata>
    {
        public new static readonly MetadataSet Empty = new MetadataSet(Enumerable.Empty<IMetadata>());

        public MetadataSet(IEnumerable<IMetadata> metadata)
            : base(GroupMetadata(metadata))
        {
        }

        private static Dictionary<SimpleName, FixedList<IMetadata>> GroupMetadata(
            IEnumerable<IMetadata> metadata)
        {
            return metadata.Distinct().GroupBy(LookupByName)
                .ToDictionary(g => g.Key, g => g.ToFixedList());
        }

        private static SimpleName LookupByName(IMetadata metadata)
        {
            return metadata.FullName.UnqualifiedName.WithoutDeclarationNumber();
        }

        IEnumerator<IMetadata> IEnumerable<IMetadata>.GetEnumerator()
        {
            return Values.SelectMany().GetEnumerator();
        }
    }
}
