using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;
using Adamant.Tools.Compiler.Bootstrap.Names;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    /// <summary>
    /// The name of a function, method, constructor that is being invoked
    /// </summary>
    public interface ICallableNameSyntax : ISyntax, IHasContainingScope
    {
        Name Name { get; }
        [DisallowNull] IFunctionSymbol? ReferencedSymbol { get; set; }
    }
}
