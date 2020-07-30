using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;
using Adamant.Tools.Compiler.Bootstrap.Types;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    [Closed(
        typeof(ISelfParameterSyntax),
        typeof(INamedParameterSyntax),
        typeof(IFieldParameterSyntax))]
    public interface IParameterSyntax : ISyntax, IBindingSymbol
    {
        SimpleName Name { get; }
        bool Unused { get; }
        new TypePromise Type { get; }
    }
}
