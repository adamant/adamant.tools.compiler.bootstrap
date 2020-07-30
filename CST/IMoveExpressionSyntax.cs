using System.Diagnostics.CodeAnalysis;
using Adamant.Tools.Compiler.Bootstrap.Metadata;

namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    /// <summary>
    /// i.e. `move name`. A move takes ownership of something from a variable
    /// </summary>
    public interface IMoveExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Referent { get; }
        [DisallowNull] IBindingMetadata? MovedSymbol { get; set; }
    }
}
