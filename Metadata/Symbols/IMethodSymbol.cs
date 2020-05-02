namespace Adamant.Tools.Compiler.Bootstrap.Metadata.Symbols
{
    public interface IMethodSymbol : IFunctionSymbol
    {
        IBindingSymbol SelfParameterSymbol { get; }
    }
}
