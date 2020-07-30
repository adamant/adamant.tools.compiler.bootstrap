using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    [Closed(
        typeof(IParentSymbol),
        typeof(IBindingSymbol))]
    public interface ISymbol
    {
        Name FullName { get; }
    }
}
