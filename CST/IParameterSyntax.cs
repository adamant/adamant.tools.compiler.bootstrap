using Adamant.Tools.Compiler.Bootstrap.Metadata;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(ISelfParameterSyntax),
        typeof(INamedParameterSyntax),
        typeof(IFieldParameterSyntax))]
    public interface IParameterSyntax : ISyntax, IBindingMetadata
    {
        SimpleName Name { get; }
        bool Unused { get; }
        new TypePromise Type { get; }
    }
}
