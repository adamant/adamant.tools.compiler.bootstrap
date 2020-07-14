namespace Adamant.Tools.Compiler.Bootstrap.FST
{
    public interface ISelfParameterSyntax : IParameterSyntax
    {
        bool MutableSelf { get; }
    }
}
