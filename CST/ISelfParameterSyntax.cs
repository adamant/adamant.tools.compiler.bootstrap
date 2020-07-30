namespace Adamant.Tools.Compiler.Bootstrap.CST
{
    public interface ISelfParameterSyntax : IParameterSyntax
    {
        bool MutableSelf { get; }
    }
}
