using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Types;
using Adamant.Tools.Compiler.Bootstrap.Names;
using ExhaustiveMatching;

namespace Adamant.Tools.Compiler.Bootstrap.AST
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
