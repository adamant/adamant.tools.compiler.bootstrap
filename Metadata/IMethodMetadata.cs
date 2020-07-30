namespace Adamant.Tools.Compiler.Bootstrap.Metadata
{
    public interface IMethodMetadata : IFunctionMetadata
    {
        IBindingMetadata SelfParameterMetadata { get; }
    }
}
