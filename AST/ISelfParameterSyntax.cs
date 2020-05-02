namespace Adamant.Tools.Compiler.Bootstrap.AST
{
    public interface ISelfParameterSyntax : IParameterSyntax
    {
        bool MutableSelf { get; }
    }
}
