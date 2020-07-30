namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    public interface IMethodSymbol : IFunctionSymbol
    {
        IBindingSymbol SelfParameterSymbol { get; }
    }
}
