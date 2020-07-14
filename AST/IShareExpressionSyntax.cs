using Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols;

namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    /// <summary>
    /// A bare variable of a reference type is generally a share of that reference.
    /// Share expressions are inserted into the AST where needed to reflect this.
    /// </summary>
    public interface IShareExpressionSyntax : IExpressionSyntax
    {
        ref IExpressionSyntax Referent { get; }
        IBindingSymbol SharedSymbol { get; }
    }
}
