using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    [Closed(
        typeof(IParentMetadata),
        typeof(IBindingMetadata))]
    public interface IMetadata
    {
        Name FullName { get; }
    }
}
