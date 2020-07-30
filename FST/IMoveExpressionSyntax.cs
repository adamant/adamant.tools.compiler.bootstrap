using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    /// <summary>
    /// i.e. `move name`. A move takes ownership of something from a variable
    /// </summary>
    public interface IMoveExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Referent { get; }
        [DisallowNull] IBindingSymbol? MovedSymbol { get; set; }
    }
}